<?php $sendBuffer = TRUE; ob_start(); ?>
<html>
<head>
<title>Cloud Storage 2022 Demos - ShareFile</title>
<link rel="stylesheet" type="text/css" href="stylesheet.css">
<meta name="description" content="Cloud Storage 2022 Demos - ShareFile">
</head>

<body>

<div id="content">
<h1>Cloud Storage - Demo Pages</h1>
<h2>ShareFile</h2>
<p>Uses the ShareFile component to manage and share files and folders.</p>
<a href="default.php">[Other Demos]</a>
<hr/>

<?php
require_once('../include/cloudstorage_sharefile.php');
require_once('../include/cloudstorage_oauth.php');
require_once('../include/cloudstorage_const.php');

?>

<?php
  session_start();
?>

<head>
<style>
.loginScreen {
  float: left;
  text-align: left;
  width: 100%;
}
</style>
</head>

<?php
  $loggedIn = isset($_SESSION["loggedIn"]) ? $_SESSION["loggedIn"] : false;
  $currentFolderID = isset($_POST["currentFolderIDInput"]) && !empty($_POST["currentFolderIDInput"]) ? $_POST["currentFolderIDInput"] : "home";
  $oauth = new CloudStorage_OAuth();
  $sharefile = new CloudStorage_ShareFile();

  // Default demo login credentials.
  $accountSubdomain = "nsoftware";
  $clientID = "Hb7kR4d6mBvQxnOe9Eg6V8CMIVjQ67vD";
  $clientSecret = "jHAZH6f1Ac5O9krW9az3fLYFEZW4KoF1sr1hqDoVgrvv1zwc";

  // Set up the local page URL.
  $pageURL = $_SERVER["HTTPS"] === "ON" ? "https://" : "http://";
  $pageURL .= $_SERVER["HTTP_HOST"];
  $pageURL .= $_SERVER["PHP_SELF"];

  /* NOTE: ShareFile requires a registered redirect URI which is https://
  *       and explicitly disallows localhost. A valid public domain with SSL enabled must
  *       be specific when creating your ClientId and ClientSecret values
  *       
  *       Visit https://api.sharefile.com/apikeys to create your credentials.
  *       
  * IMPORTANT: 
  *      Because of the ShareFile restrictions detailed above this demo
  *      make use of an existing public URL (oauth.nsoftware.com)
  *      This should NOT be used in production. It is only used here to
  *      facilitate running of this demo. In a real application you should
  *      register your own Redirect URI.
  *      
  *      For any questions please contact support@nsoftware.com.
  */
  $returnURL = "https://oauth.nsoftware.com/oauthdemo";
?>

<?php
    $exception = "";
    try {
      // Login button pressed.
      if (isset($_POST["loginButton"])) {
        // Set component information and redirect to authorization URL.
        $oauth->setClientProfile(1);
        if (isset($_POST["clientIDInput"])) {
          $oauth->setClientId($_POST["clientIDInput"]);
          $_SESSION["clientIDInput"] = $_POST["clientIDInput"];
        }
        if (isset($_POST["clientSecretInput"])) {
          $oauth->setClientSecret($_POST["clientSecretInput"]);
          $_SESSION["clientSecretInput"] = $_POST["clientSecretInput"];
        }
        $oauth->setServerAuthURL("https://secure.sharefile.com/oauth/authorize");
        if (isset($_POST["accountSubdomainInput"])) {
          $oauth->setServerTokenURL("https://" . $_POST["accountSubdomainInput"] . ".sharefile.com/oauth/token");
          $_SESSION["accountSubdomainInput"] = $_POST["accountSubdomainInput"];
        }
        $oauth->setReturnURL($returnURL);
        $oauth->doAddParam("state", $pageURL);
        header("Location: " . $oauth->doGetAuthorizationURL());
        exit();
      }
      else if (isset($_GET["code"]) && empty($_POST)) {
        // Redirected from authorization page.  Get authorization string and complete login process.
        $oauth->setClientProfile(1);
        $oauth->setClientId($_SESSION["clientIDInput"]);
        $oauth->setClientSecret($_SESSION["clientSecretInput"]);
        $oauth->setServerAuthURL("https://secure.sharefile.com/oauth/authorize");
        $oauth->setServerTokenURL("https://" . $_SESSION["accountSubdomainInput"] . ".sharefile.com/oauth/token");
        $oauth->setAuthorizationCode($_GET["code"]);
        $sharefile->setAccountSubdomain($_SESSION["accountSubdomainInput"]);
        $sharefile->setAuthorization($oauth->doGetAuthorization());
        $loggedIn = true;
        $_SESSION["authString"] = $sharefile->getAuthorization();
        $_SESSION["loggedIn"] = true;
      }
    }
    catch (Exception $e) {
      $exception = $e->getMessage();
    }
    finally {
      if (!$loggedIn) {
        // Prompt the user to provide their OAuth information.
        echo "<div class='loginScreen'>";
        echo "<form method='post'>";
		echo "<p>This demo shows how to list, upload, download, delete, and share documents from ShareFile.  To begin, click Login to allow the application to access your account.</p>";

        echo "<table>";

        echo "<tr>";
        echo "<td><label for='accountSubdomainInput'>Account Subdomain: &emsp;&ensp;</label>";
        if (isset($_SESSION["accountSubdomainInput"])) {
          echo "<input type='text' id='accountSubdomainInput' name='accountSubdomainInput' size='60' value='" . $_SESSION["accountSubdomainInput"] . "'>";
        }
        else {
          echo "<input type='text' id='accountSubdomainInput' name='accountSubdomainInput' size='60' value='" . $accountSubdomain . "'>";
        }
        echo "</td></tr>";
      
        echo "<tr>";
        echo "<td><label for='clientIDInput'>Client ID: &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;</label>";
        if (isset($_SESSION["clientIDInput"])) {
          echo "<input type='text' id='clientIDInput' name='clientIDInput' size='60' value='" . $_SESSION["clientIDInput"] . "'>";
        }
        else {
          echo "<input type='text' id='clientIDInput' name='clientIDInput' size='60' value='" . $clientID . "'>";
        }
        echo "&emsp;Obtain and set your Client ID and Client Secret, which can both be found in the <a href='https://code.google.com/apis/console#access'>API Console</a>.<br/>";
        echo "</td></tr>";

        echo "<tr>";
        echo "<td><label for='clientSecretInput'>Client Secret: &emsp;&emsp;&emsp;&emsp;&ensp;</label>";
        if (isset($_SESSION["clientSecretInput"])) {
          echo "<input type='text' id='clientSecretInput' name='clientSecretInput' size='60' value='" . $_SESSION["clientSecretInput"] . "'>";
        }
        else {
          echo "<input type='text' id='clientSecretInput' name='clientSecretInput' size='60' value='" . $clientSecret . "'>";
        }
        echo "</td></tr>";

        echo "<tr>";
        echo "<td><input type='submit' id='loginButton' name='loginButton' value='  Login  '></td><br/><br/>";
        echo "</tr>";
        
        echo "</table>";

        echo "<font color='red'>" . $exception . "</font>";
        echo "</form>";
        echo "</div>";
      }
    }
?>

<form method="post">
  <input type="submit" id="logoutButton" name="logoutButton" value="  Logout  "
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  <br/><br/>
  <input type="submit" id="listButton" name="listButton" value="  List Documents  "
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="currentFolderIDInput" name="currentFolderIDInput" size="35" placeholder="Folder ID to View"
    <?php
      if ($loggedIn) {
        // Home directory is default location so no need to show in this case; better to display placeholder message instead.
        if ($currentFolderID != "home") {
          echo "value='" . $currentFolderID . "'";
        }
      }
      else {
        echo "hidden";
      }
    ?>
  >
  &emsp;&emsp;
  <input type="submit" id="createButton" name="createButton" value="  Create Folder  "
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="createNameInput" name="createNameInput" size="35" placeholder="Desired Folder Name"
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &emsp;&emsp;
  <input type="submit" id="deleteButton" name="deleteButton" value="  Delete Item  "
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="deleteIDInput" name="deleteIDInput" size="35" placeholder="Item ID to Delete"
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  <br/><br/>
  <input type="submit" id="uploadButton" name="uploadButton" value="  Upload File  "
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="uploadPathInput" name="uploadPathInput" size="35" placeholder="Full File Path"
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="uploadNameInput" name="uploadNameInput" size="35" placeholder="Desired File Name (include extension)"
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &emsp;&emsp;
  <input type="submit" id="downloadButton" name="downloadButton" value="  Download File  "
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="downloadPathInput" name="downloadPathInput" size="35" placeholder="Full File Path"
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="downloadIDInput" name="downloadIDInput" size="35" placeholder="File ID to Download"
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  <br/><br/>
  <input type="submit" id="requestButton" name="requestButton" value="  Request Link  "
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="requestIDInput" name="requestIDInput" size="35" placeholder="Folder ID for Link"
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &emsp;&emsp;
  <input type="submit" id="shareButton" name="shareButton" value="  Share Link  "
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
  &ensp;
  <input type="text" id="shareIDInput" name="shareIDInput" size="35" placeholder="Item ID(s) to Include (comma-separated)"
    <?php
      if (!$loggedIn) {
        echo "hidden";
      }
    ?>
  >
</form>

<?php
  if ($loggedIn) {
    try {
      // Reauthenticate component on page reload.
      if (empty($sharefile->getAuthorization())) {
        if (isset($_SESSION["authString"])) {
          $sharefile->setAuthorization($_SESSION["authString"]);
        }
      }
      // Respecify subdomain on page reload.
      if (empty($sharefile->getAccountSubdomain())) {
        if (isset($_SESSION["accountSubdomainInput"])) {
          $sharefile->setAccountSubdomain($_SESSION["accountSubdomainInput"]);
        }
      }
      // Logout button pressed.
      if (isset($_POST["logoutButton"])) {
        $oauth->doReset();
        $sharefile->doReset();
        $_SESSION["loggedIn"] = false;
        header("Location: " . $pageURL);
      }
      // Upload File button pressed.
      else if (isset($_POST["uploadButton"])) {
        $sharefile->setLocalFile($_POST["uploadPathInput"]);
        $sharefile->doUploadFile($_POST["uploadNameInput"], $currentFolderID);
      }
      // Download File button pressed.
      else if (isset($_POST["downloadButton"])) {
        $sharefile->setLocalFile($_POST["downloadPathInput"]);
        $sharefile->doDownloadFile($_POST["downloadIDInput"]);
      }
      // Delete Item button pressed.
      else if (isset($_POST["deleteButton"])) {
        $sharefile->doDeleteItem($_POST["deleteIDInput"]);
      }
      // Create Folder button pressed.
      else if (isset($_POST["createButton"])) {
        $sharefile->doCreateFolder($_POST["createNameInput"], $currentFolderID);
      }
      // Request Link button pressed.
      else if (isset($_POST["requestButton"])) {
        $requestLink = $sharefile->doCreateRequestLink($_POST["requestIDInput"]);
        echo "Request Link:  <a href='" . $requestLink . "' target='_blank'>" . $requestLink . "</a>.  Send this link to other parties to allow them to upload to the specified folder.<br/><br/>";
      }
      // Share Link button pressed.
      else if (isset($_POST["shareButton"])) {
        $shareLink = $sharefile->doCreateLink($_POST["shareIDInput"]);
        echo "Share Link:  <a href='" . $shareLink . "' target='_blank'>" . $shareLink . "</a>.  Send this link to other parties to grant access to the item(s).<br/><br/>";
      }

      // Display information.
      echo "<div>";

      echo "<table style='text-align:center;width:100%'>";

      echo "<colgroup>";
      echo "<col style='width:25%'>";
      echo "<col style='width:15%'>";
      echo "<col style='width:30%'>";
      echo "<col style='width:30%'>";
      echo "</colgroup>";

      echo "<thead>";
      echo "<tr>";
      echo "<th>Title</th>";
      echo "<th>Type</th>";
      echo "<th>Creation Date</th>";
      echo "<th>ID</th>";
      echo "</tr>";
      echo "</thead>";

      echo "<tbody>";
      // The first row shows the parent of the current directory.
      $parentID = "";
      $sharefile->doGetItemInfo($currentFolderID);
      if ($sharefile->getItemCount() > 0 && !empty($sharefile->getItemParentId(0))) {
        $parentID = $sharefile->getItemParentId(0);
      }
      $sharefile->doListItems($currentFolderID);
      if (!empty($parentID)) {
        echo "<tr>";
        echo "<td>..</td>";
        echo "<td>Folder</td>";
        echo "<td>-</td>";
        echo "<td>" . $parentID . "</td>";
        echo "</tr>";
      }
      // Subsequent rows show the contents of the current directory.
      for ($itemIndex = 0; $itemIndex < $sharefile->getItemCount(); $itemIndex++) {
        echo "<tr>";
        echo "<td>" . $sharefile->getItemName($itemIndex) . "</td>";
        resolveItemType($sharefile->getItemType($itemIndex));
        echo "<td>" . $sharefile->getItemCreationDate($itemIndex) . "</td>";
        echo "<td>" . $sharefile->getItemId($itemIndex) . "</td>";
        echo "</tr>";
      }
      echo "</tbody>";

      echo "</table>";

      echo "</div>";
    }
    catch (Exception $e) {
      echo "<font color='red'>" . $e->getMessage() . "</font><br/><br/>";
    }
  }

  function resolveItemType($itemType) {
    switch ($itemType) {
      case 0:
        echo "<td>File</td>";
        break;
      case 1:
        echo "<td>Folder</td>";
        break;
      case 2:
        echo "<td>Link</td>";
        break;
      case 3:
        echo "<td>Note</td>";
        break;
      default:
        echo "<td>Unknown</td>";
    }
  }
?>

</div>


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
