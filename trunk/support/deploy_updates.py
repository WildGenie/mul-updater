#!/usr/bin/python
# -*- coding: utf-8 -*-

# deploy.py
#
# This script archives and uploads updates to your web server.
# Put all files that you want to be uploaded to the same directory
# with this script
#
# Note: configure FTP settings in deploy.conf prior to running it
# 
# by AnDenixa, Fall 2013
# Note: must be run in the folder containing files needs to be uploaded
#
# 5 October, 2012: No longer requires 7zip as external packer


import sys
import hashlib
import zipfile
import ftplib
import ConfigParser
import socket


from multiprocessing import Pool, cpu_count
from os import listdir, getcwd, stat, remove
from os.path import isfile, join, basename, splitext
from time import clock


def compress( zip_name, filename, compression = 5 ):
    
    sys.stdout.write('Compressing: {}\n'.format(basename(filename)))
    sys.stdout.flush()    
    file = zipfile.ZipFile(zip_name, "w")    
    
    base_filename = basename(filename);
    
    file_obj = open(filename, 'rb')

    file_md5 = hashlib.md5(file_obj.read()).hexdigest()    
    file_obj.close()    
    
    file.writestr(base_filename+".md5", file_md5, zipfile.ZIP_STORED);    
    file.write(filename, base_filename, zipfile.ZIP_DEFLATED)
    
    file.close()

    
def draw_ascii_progress(percent):
    sys.stdout.write('\r')        
    sys.stdout.write(("[{:<20}] {}%").format('='*int(((percent/100.0)*20)), percent))
    sys.stdout.flush()
        
    
def show_upload_progress(p):    
    global total_bytes_uploaded
    total_bytes_uploaded += len(p)    
    percent = int((total_bytes_uploaded*100) / filesize)     
    draw_ascii_progress(percent)           
        
    
def main():        
    global filesize, total_bytes_uploaded
    
    working_dir = getcwd()    
    
    if not isfile(join(working_dir, "deploy.conf")):
        print "Can't find deploy.conf"
        return -1
    
    cfg = ConfigParser.ConfigParser()
    cfg.read("deploy.conf")
        
    include_ext = [ext.lower() if ext[0]=="." else "."+ext.lower() for ext in cfg.get("settings", "INCLUDE_FILE_EXT").split() if not "*" in ext]            
    file_list = [f for f in listdir(working_dir) if isfile(join(working_dir, f)) and splitext(f)[1].lower() in include_ext]    
                
    proc_num = cpu_count()        
    if proc_num > len(file_list): proc_num = len(file_list)        
    pool = Pool(processes = proc_num)      
    
    archive_names = []
    
    for file_name in file_list:
                                             
        achive_name = file_name + ".zip"
                
        res = pool.apply_async(compress, args = (achive_name, file_name,))
        
        archive_names.append(achive_name)
           
        if cfg.getboolean("settings", "CREATE_BUNDLE"): 
            try:
                remove("updates.zip")    
            except:
                pass                           
            compress( "updates.zip", file, cfg.getint("settings", "COMPRESS_RATIO") )
                
    res.get()    
    pool.close()
    pool.join()                
                
    print "\nConnecting to {} ..".format(cfg.get("settings", "FTP_ADDRESS"))
    
    session = ftplib.FTP(cfg.get("settings", "FTP_ADDRESS"), 
                         cfg.get("settings", "FTP_USERNAME"),
                         cfg.get("settings", "FTP_PASSWORD"))
    
    session.cwd(cfg.get("settings", "UPDATES_URL"))       
    print "Remote dir is {}\n".format(session.pwd())
                             
    for arch_name in archive_names:
        
        statinfo = stat(arch_name)
        filesize = statinfo.st_size
        
        total_bytes_uploaded = 0
        
        print "Uploading {} ..".format(arch_name)
        
        with open(arch_name, 'rb') as f:
            when = clock()        
            try:
                session.delete(arch_name)
            except:
                pass
                
            try:
                session.storbinary('STOR ' + arch_name, f, callback=show_upload_progress)     # send the file
            except socket.error as e:
                print e
                return -1
                  
            speed = filesize / (clock() - when)         
            print "\nUploading complete (%0.2f KB/sec)\n" % (speed / 1024)                    
            
        remove(arch_name)
        
    print "Closing connection."    
    # close file and FTP            
    session.quit()


if __name__ == '__main__':
    sys.exit(main())