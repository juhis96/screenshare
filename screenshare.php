<?php 
if (count($_POST)){
	$imageData = base64_decode($_POST['imgData']);
	$img = fopen('img.jpg', 'w');
	fwrite($img, $imageData);
	fclose($img);
	exit('Success');
}

if (file_exists('img.jpg')){
	echo '<img src="img.jpg" id="img"/>';
	echo "<script>setInterval(function() {
 var myImageElement = document.getElementById('img');
 myImageElement.src = 'img.jpg?rand=' + Math.random();
}, 2500);</script>";
}else{
	echo "Offline";
}
?>