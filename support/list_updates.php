<?php
	
////
// list_updates.php
//
// Put this to your web server folder 
// containing update files.
//
// By AnDenixa, Fall 2013
// Zuluhotel.net files updater
////


$skip_names = array( basename( __FILE__ ), ".", "..", "error_log", ".htaccess" );

if ($handle = opendir('.')) {
    while (false !== ($filename = readdir($handle))) {
        if (!in_array($filename, $skip_names)) {
			$ext = pathinfo($filename, PATHINFO_EXTENSION);
			if ($ext=="zip") {
            	echo "$filename,".zipfile_filesize( $filename )
            		.",".zipfile_crc32($filename)
            		.PHP_EOL;				
			} else {
            	echo "$filename,".filesize ( $filename )
            		.",".hash_file("crc32b", $filename)
            		.PHP_EOL;
            }                        
        }
    }
    closedir($handle);
}

function zipfile_filesize( $filename )
{
	$zip = new ZipArchive;		
	$res = $zip->open($filename);
	$filename = basename($filename, ".zip");
	$stats = $zip->statName($filename, ZipArchive::FL_NOCASE | ZipArchive::FL_NODIR);
	$result = $stats["size"];
	$zip->close();
	return $result;
}

function zipfile_crc32( $filename )
{
	$zip = new ZipArchive;		
	$res = $zip->open($filename);
	$filename = basename($filename, ".zip");
	$stats = $zip->statName($filename, ZipArchive::FL_NOCASE | ZipArchive::FL_NODIR);
	$result = dechex($stats["crc"]);
	$zip->close();
	return $result;
}
?>