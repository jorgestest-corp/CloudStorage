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

main();

async function main(){
  const s3 = new cloudstorage.amazons3();
  s3.on("SSLServerAuthentication", (e) => (e.accept = true));

  console.log("\nWelcome to the Amazon S3 Demo.");
  console.log("This demo shows how to use the Cloud Storage to interact with Amazon S3. Amazon's Simple Storage Service."); 
  console.log("It is assumed that you already signed up for the service (at aws.amazon.com/s3) and have an Access Key and Secret Key which are required to use the service \n");
  console.log("------------------------------------------------------------ \n");

  prompt("accesskey", "AWS Access Key", ":", "");

  rl.on('line', async (line) => {
    switch (lastPrompt) {
      case "accesskey":
        s3.setAccessKey(line);
        prompt("secretkey", "AWS Secret Key", ":", "");
      break;
      case "secretkey":
        s3.setSecretKey(line);
        //List all the buckets
        console.log("\nBucket List:\n");
        try{
          //Async function, captured by "BucketList" event
          await s3.listBuckets();
        } catch (error){
          console.log(error.message);      
          process.exit(1);
        }
        //Sends to no case, which will allow default case in following switch case to print menus
        lastPrompt = ""		
      break;
      //Used for moving into a bucket.
      case "bucket" :
        //Wait for confirmation that the bucket was set
        await s3.setBucket(line);
        console.log(`Changed to bucket: ${s3.getBucket()}`)
        //Print menus
        lastPrompt = "";
      break;
      //Used to get a file from a bucket
      case "file":
        try { 
          //Wait for its return
          await s3.getObject(line);
        } catch (error) {
          console.log(error.message);
          process.exit(1);
        }  
        objectdata = s3.getObjectData();
        console.log(`The content of the file: \r\n ${objectdata}`);
        //Print menus
        lastPrompt = ""		
      break;
      //Used to put a file in a bucket, sets the local file name
      case "local_file" :
        try{
          //Sets where the local file is
          s3.setLocalFile(line);
        } catch (error) {
          console.log(error.message);
          process.exit(1);
        }          
        //Asks what the name of the new object will be, sets start to "rem_object"
        prompt("rem_object", "Name of new object?", ":", ""); 
      break;
      //Used to put a file in a bucket, sets the remote file name
      case "rem_object" :
        try{
          //Wait to finish
          await s3.createObject(line);
        } catch (error){
          console.log(error.message);
          process.exit(1);
        }
        console.log("Object Created");
        s3.setLocalFile("");
        //print menus
        lastPrompt = "";
      break;    
    }
    switch (line) {
      case "?":
        printMenu();
        menuPrompt();
      break;
      case "cd":
        prompt("bucket", "Which Bucket?", ":", "");
      break;
      case "ls": 
        try{
          console.log("Objects: ");
          //Async function, captured by the "ObjectList" event
          await s3.listObjects();
        } catch (error){
          console.log(error.message);
          process.exit();
        }
        printMenu();
        menuPrompt();       
      break;
      case "get" :
        prompt("file", "Which File?", ":", "");
      break;
      case "put" :
        prompt("local_file", "Local file to upload", ":", "");
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
  });
  s3.on("BucketList", async (e)=>(console.log(e.bucketName)))
  .on("ObjectList", async (e) => (console.log(e.objectName)));
}

function printMenu() {
  console.log("\n?     -   Help");
  console.log("cd    -   Change to bucket");
  console.log("ls    -   List objects");
  console.log("get   -   Get remoteFile");
  console.log("put   -   Put localFile");
  console.log("q     -   Quit\n");	
  lastPrompt = "";
}
function menuPrompt(){
  process.stdout.write('Enter command: ');
  lastPrompt = "";  
}


function prompt(promptName, label, punctuation, defaultVal)
{
  lastPrompt = promptName;
  lastDefault = defaultVal;
  process.stdout.write(`${label} [${defaultVal}] ${punctuation} `);
}
