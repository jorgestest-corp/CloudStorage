/*
 * Cloud Storage 2022 JavaScript Edition - Sample Project
 *
 * This sample project demonstrates the usage of Cloud Storage in a 
 * simple, straightforward way. It is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * www.nsoftware.com/cloudstorage
 *
 * This code is subject to the terms and conditions specified in the 
 * corresponding product license agreement which outlines the authorized 
 * usage and restrictions.
 */
 
const readline = require("readline");
const cloudstorage = require("@nsoftware/cloudstorage");

if(!cloudstorage) {
  console.error("Cannot find cloudstorage.");
  process.exit(1);
}
let rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

const path = require("path"); // Retrieves information about passed paths.
const fs = require("fs"); // Ensures that a provided file is valid.
let lastPrompt = ""; // The key for the last prompt sent to the user.
let lastDefault = ""; // The last default for the last prompt sent to the user.
let currentFolderId = ""; // The ID of the folder that the user is currently in.
let currentParentId = ""; // The ID of the parent to the folder that the user is currently in.
let subdomain = ""; // The account subdomain for the currently authenticated user.
let authStr = ""; // The auth string for the currently authenticated user.
const sharefile = new cloudstorage.sharefile();
const oauth = new cloudstorage.oauth();
main();


async function main(){


  console.log("\n*****************************************************************");
  console.log("\n* This demo shows how to use the ShareFile component to list,   *");
  console.log("\n* upload, download, delete, and share documents from ShareFile. *");
  console.log("\n* Default prompt values are in [ ] where applicable.            *");
  console.log("\n*****************************************************************\n");

    /*
    * NOTE: ShareFile requires a registered redirect URI which is https://
    *       and explicitly disallows localhost. A valid public domain with SSL enabled must
    *       be specific when creating your ClientId and ClientSecret values
    *    
    *       Visit https://api.sharefile.com/apikeys to create your credentials
          
    * IMPORTANT: 
    *       Because of the ShareFile restrictions detailed above this demo
    *       make use of an existing public URL (oauth.nsoftware.com)
    *       This should NOT be used in production. It is only used here to
    *       facilitate running of this demo. In a real application you should
    *       register your own Redirect URI.
    *      
    *       For any questions please contact support@nsoftware.com 
    */

  prompt("subdomain", "Account Subdomain", ":", "");
  rl.on('line', async (line) => {
    try{
      switch (lastPrompt) {
        // Enter account subdomain.
        case "subdomain":
          if(line === ""){
            subdomain = lastDefault;
          } else {
            subdomain = line;
          }
          prompt("client_id", "Client ID", ":", "Hb7kR4d6mBvQxnOe9Eg6V8CMIVjQ67vD");
        break;
        // Enter OAuth client ID.
        case "client_id":
          if(line === ""){
            oauth.setClientId(lastDefault);
          } else {
            oauth.setClientId(line);
          }
          prompt("client_secret", "Client Secret", ":", "jHAZH6f1Ac5O9krW9az3fLYFEZW4KoF1sr1hqDoVgrvv1zwc");
        break;
        // Enter OAuth client secret.
        case "client_secret" :
          if(line === ""){
            oauth.setClientSecret(lastDefault);
          } else {
            oauth.setClientSecret(line);
          }
          oauth.setClientProfile(cloudstorage.OauthClientProfiles.ocpApplication);
          oauth.setServerAuthURL("https://secure.sharefile.com/oauth/authorize");
          oauth.setServerTokenURL(`https://${subdomain}.sharefile.com/oauth/token`);
          
          oauth.setWebServerPort(7878); // Specify a particular port, as it will be used in the redirect.
          await oauth.startWebServer(); // In most cases, the webserver is automatically started when getAuthorization is called.  In this case, due to the use of oauth.nsoftware.com, it is started manually.
          oauth.setReturnURL("https://oauth.nsoftware.com/oauthdemo"); // This URL should NOT be used in a production application.  It is only for testing purposes.
          oauth.addParam("state", `http://localhost:${oauth.getWebServerPort()}`); // The oauth.nsoftware.com server uses this value to redirect to the local server embedded within the OAuth class.

          //oauth.setRefreshToken(""); // Set to value from previous runs to expedite the login process.
          authStr = await oauth.getAuthorization();
          //console.log(oauth.getRefreshToken()); // RefreshToken may be used to automatically obtain a new token without user interaction.
          await displayList("home");
          lastPrompt = "";
        break;
        // Retrieve number corresponding to the folder the user wishes to view.
        case "cd":
          let cd_choice = parseInt(line);
          if(cd_choice === NaN){
            console.log("Please choose a valid number corresponding to a folder.");
            lastPrompt = "";
            break;
          }
          if(cd_choice === -1){
            await displayList(currentParentId);
          } else {
            await displayList(sharefile.getItems().get(cd_choice).getId());
          }
          lastPrompt = "";
        break;
        // Retrieve download location.
        case "get_location" :
          sharefile.setLocalFile(line);
          prompt("get_file","Number corresponding to item you wish to download", ":", "" );
        break;
		// Retrieve number corresponding to the item the user wishes to download.
        case "get_file" :
          let get_choice = parseInt(line);
          if(get_choice === NaN){
            console.log("Please choose a valid number corresponding to an item.");
            lastPrompt = "";
            break;
          }
          if(sharefile.getItems().get(get_choice).getType() === cloudstorage.ShareFileItemTypes.sfitFile){
            sharefile.setLocalFile(sharefile.getLocalFile() + sharefile.getItems().get(get_choice).getName());
          } else if(sharefile.getItems().get(get_choice).getType() === cloudstorage.ShareFileItemTypes.sfitFolder){
            sharefile.setLocalFile(sharefile.getLocalFile() + sharefile.getItems().get(get_choice).getName() + ".zip");
          } else {
            console.log("Item is of an invalid type.");
            lastPrompt = "";
          }
          await sharefile.downloadFile(sharefile.getItems().get(get_choice).getId());
          lastPrompt = "";
        break;
		// Retrieve number corresponding to the file the user wishes to upload.
        case "put" :
          if(!fs.existsSync(line)){
            console.log("Please input valid file.");
            lastPrompt = "";
            break;
          }

          sharefile.setLocalFile(line);
          await sharefile.uploadFile(path.parse(line).base, currentFolderId);
          await displayList(currentFolderId);
          lastPrompt = "";
        break;
        // Retrieve number corresponding to the item the user wishes to delete.
        case "del" :
          let del_choice = parseInt(line)
          if (del_choice === NaN){
            console.log("Please choose a valid number corresponding to an item.");
            lastPrompt = "";
            break;
          }
          await sharefile.deleteItem(sharefile.getItems().get(del_choice).getId());
          await displayList(currentFolderId);
          lastPrompt = "";
        break;
        // Retrieve new folder name.
        case "mkdir" :
          await sharefile.createFolder(line, currentFolderId);
          await displayList(currentFolderId);
          lastPrompt = "";
        break;
        // Retrieve number corresponding to the item the user wishes to share.
        case "shr" :
          let shr_choice = parseInt(line)
          if (shr_choice === NaN){
            console.log("Please choose a valid number corresponding to an item.");
            lastPrompt = "";
            break;
          }
          console.log(`Share Link: ${await sharefile.createLink(sharefile.getItems().get(shr_choice).getId())}`);
          lastPrompt = "";
        break;
      }
      switch (line) {
        case "?":
          printMenu();
          menuPrompt();
        break;
        case "cd":
          prompt("cd", "Number corresponding to folder you wish to view (-1 to go up a level)", ":", "");
        break;
        case "ls": 
          await displayList(currentFolderId)
          printMenu();
          menuPrompt();       
        break;
        case "get" :
          prompt("get_location", "Directory in which to download", ":", "");
        break;
        case "put" :
          prompt("put", "Path of file that will be uploaded", ":", "");
        break;
        case "del" :
          prompt("del", "Number corresponding to item you wish to delete", ":", "");
        break;
        case "mkdir" :
          prompt("mkdir", "Name of new folder", ":", "");
        break;
        case "shr" :
          prompt("shr", "Number corresponding to item you wish to share", ":", "");
        break;
        case "req" :
          console.log(`Request Link: ${await sharefile.createRequestLink(currentFolderId)}`);
          printMenu();
          menuPrompt();
        break;
        case "q":
          process.exit();
        break;
        default:
        if (lastPrompt === ""){
          printMenu();
          menuPrompt();
        }
        break;
      }
    } catch (error){
      console.log(error.message);      
      process.exit(1);
    }
  });

}

async function displayList(folderId){
  sharefile.setAccountSubdomain(subdomain);
  sharefile.setAuthorization(authStr);

  await sharefile.getItemInfo(folderId);
  currentFolderId = sharefile.getItems().get(0).getId();
  currentParentId = sharefile.getItems().get(0).getParentId();
  
  console.log("\n",  `#  |  Title  |  Type  |  Creation Date`);
  console.log( `-1 |  ..  |  Folder  |  ..`);

  await sharefile.listItems(currentFolderId);
  let i = 0;
  for(const item of sharefile.getItems()){
    console.log(`${i}  |  ${item.getName()}  |  ${resolveItemType(item.getType())}  |  ${item.getCreationDate()}`);
    i = i + 1;
  }
}

function resolveItemType(type){
  switch (type){
    case cloudstorage.ShareFileItemTypes.sfitFile:
      return "File";
    case cloudstorage.ShareFileItemTypes.sfitFolder:
      return "Folder";
    case cloudstorage.ShareFileItemTypes.sfitLink:
      return "Link";
    case cloudstorage.ShareFileItemTypes.sfitNote:
      return "Note";
    case cloudstorage.ShareFileItemTypes.sfitSymbolicLink:
      return "Symbolic Link";
    default:
      return "";
  }
}

function printMenu() {
  console.log("\n?     -   Help");
  console.log("cd    -   Change Folders");
  console.log("ls    -   List Items");
  console.log("get   -   Download Item");
  console.log("put   -   Upload File");
  console.log("del   -   Delete Item");
  console.log("mkdir -   Create Folder");
  console.log("shr   -   Create Share Link");
  console.log("req   -   Create Request Link");
  console.log("q     -   Quit\n");	
  lastPrompt = "";
}
function menuPrompt(){
  process.stdout.write('>: ');
  lastPrompt = "";  
}


function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
