<?php $sendBuffer = TRUE; ob_start(); ?>
<html>
<head>
<title>Cloud Storage 2022 Demos - Google Drive Browser</title>
<link rel="stylesheet" type="text/css" href="stylesheet.css">
<meta name="description" content="Cloud Storage 2022 Demos - Google Drive Browser">
</head>

<body>

<div id="content">
<h1>Cloud Storage - Demo Pages</h1>
<h2>Google Drive Browser</h2>
<p>Uses the GoogleDrive component to manage the documents in Google Drive.</p>
<a href="default.php">[Other Demos]</a>
<hr/>

<?php
require_once('../include/cloudstorage_googledrive.php');
require_once('../include/cloudstorage_oauth.php');
require_once('../include/cloudstorage_const.php');

?>

<?php
  $googledrive1 = new CloudStorage_GoogleDrive();
?>
<form method=POST>
<center>
<table width="90%">

 <tr><td>OAuth Client Id:        <td><input type=text name=clientId size=40>
 <tr><td>OAuth Client Secret:    <td><input type=password name=clientSecret size=40>

 <tr><td><td><input type=submit value="List Documents">

</table>
</center>
</form>

<?php

// Setup the ReturnURL
if ($_SERVER['HTTPS'] == 'ON') {
  $pageUrl = 'https://';
} else {
  $pageUrl = 'http://';
}
$pageUrl .= $_SERVER['HTTP_HOST'];
$pageUrl .= $_SERVER['PHP_SELF'];

try{
	if ($_SERVER['REQUEST_METHOD'] == "POST") {

		if (array_key_exists("clientId", $_POST))
  	  {
	  	// Add these values to cookies
	  	setcookie('clientId', $_POST['clientId'], time() + 900);  
	  	setcookie('clientSecret', $_POST['clientSecret'], time() + 900);
	  
	  	$googledrive1->setOAuthClientProfile(1);
		$googledrive1->doConfig("OAuthUsePKCE = false");
	  	$googledrive1->setOAuthClientId($_POST['clientId']);
	  	$googledrive1->setOAuthClientSecret($_POST['clientSecret']);
	  	$googledrive1->setOAuthServerAuthURL('https://accounts.google.com/o/oauth2/auth');
	  	$googledrive1->setOAuthServerTokenURL('https://accounts.google.com/o/oauth2/token');
	  	$googledrive1->setOAuthAuthorizationScope('https://www.googleapis.com/auth/drive');
	  	$googledrive1->setOAuthReturnURL($pageUrl);
	  	$url = $googledrive1->getOAuthWebAuthURL();
	  	header('Location: '.$url);
	  	exit();
			} 
	}
	else
	{
		// Redirected from authorization page. Get authorization string and retrieve user information. 
		if (array_key_exists('code', $_GET))
		{
	  	$googledrive1->setOAuthClientProfile(1);
		$googledrive1->doConfig("OAuthUsePKCE = false");
	  	$googledrive1->setOAuthClientId($_COOKIE['clientId']);
	  	$googledrive1->setOAuthClientSecret($_COOKIE['clientSecret']);
	  	$googledrive1->setOAuthServerAuthURL('https://accounts.google.com/o/oauth2/auth');
	  	$googledrive1->setOAuthServerTokenURL('https://accounts.google.com/o/oauth2/token');
	  	$googledrive1->setOAuthAuthorizationScope('https://www.googleapis.com/auth/drive');
	    $googledrive1->setOAuthAuthorizationCode($_GET['code']);
	  	$googledrive1->setOAuthReturnURL($pageUrl);
	  	$googledrive1->doAuthorize();
	  	$googledrive1->doListResources();
?>

<center>
<table width="90%">
  <tr>
    <th>File Name</th>
    <th>Author</th>
    <th>Modification Time</th>
  </tr>

<?php
  		for($i= 0; $i < $googledrive1->getResourceCount(); $i++)
  		{
  			$googledrive1->setResourceId($i, $i);
?>
  <tr>
    <td nowrap><?php echo htmlspecialchars($googledrive1->getResourceName($i)); ?></td>
  	<td nowrap><?php echo htmlspecialchars($googledrive1->getResourceOwner($i)); ?></td>
    <td nowrap><?php echo htmlspecialchars($googledrive1->getResourceModifiedTime($i)); ?></td>
  </tr>

<?php
  		}
  	}  
  }
?>

</table>
</center>

<?php
}
catch (Exception $e) {
	echo 'Error: ',  $e->getMessage(), "\n";
} 

?>


<br/>
<br/>
<br/>
<hr/>
NOTE: These pages are simple demos, and by no means complete applications.  They
are intended to illustrate the usage of the Cloud Storage objects in a simple,
straightforward way.  What we are hoping to demonstrate is how simple it is to
program with our components.  If you want to know more about them, or if you have
questions, please visit <a href="http://www.nsoftware.com/?demopg-ESPHA" target="_blank">www.nsoftware.com</a> or
contact our technical <a href="http://www.nsoftware.com/support/">support</a>.
<br/>
<br/>
Copyright (c) 2023 /n software inc.
<br/>
<br/>
</div>

<div id="footer">
<center>
Cloud Storage 2022 - Copyright (c) 2023 /n software inc. - For more information, please visit our website at <a href="http://www.nsoftware.com/?demopg-ESPHA" target="_blank">www.nsoftware.com</a>.
</center>
</div>

</body>
</html>

<?php if ($sendBuffer) ob_end_flush(); else ob_end_clean(); ?>
