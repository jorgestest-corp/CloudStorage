/*
 * Cloud Storage 2022 Java Edition - Sample Project
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

import java.io.*;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import cloudstorage.*;

public class sharefile extends ConsoleDemo {
    Sharefile sharefile1 = new Sharefile();
    Oauth oauth1 = new Oauth();
    String currentFolderId = "";
    String subdomain = "";

    public sharefile() {

        String buffer = "";
        try {
			System.out.println("\n*****************************************************************");
			System.out.println("\n* This demo shows how to use the ShareFile component to list,   *");
			System.out.println("\n* upload, download, delete, and share documents from ShareFile. *");
			System.out.println("\n* Default prompt values are in [ ] where applicable.            *");
			System.out.println("\n*****************************************************************\n");
			
            /* NOTE: ShareFile requires a registered redirect URI which is https://
            *       and explicitly disallows localhost. A valid public domain with SSL enabled must
            *       be specific when creating your ClientId and ClientSecret values
            *       
            *       Visit https://api.sharefile.com/apikeys to create your credentials
            *       
            * IMPORTANT: 
            *      Because of the ShareFile restrictions detailed above this demo
            *      make use of an existing public URL (oauth.nsoftware.com)
            *      This should NOT be used in production. It is only used here to
            *      facilitate running of this demo. In a real application you should
            *      register your own Redirect URI.
            *      
            *      For any questions please contact support@nsoftware.com
            */

            subdomain = prompt("Account Subdomain",":","");

            oauth1.setClientProfile(0); // Application
            oauth1.setClientId(prompt("OAuth Client ID",":","Hb7kR4d6mBvQxnOe9Eg6V8CMIVjQ67vD"));
            oauth1.setClientSecret(prompt("OAuth Client Secret",":","jHAZH6f1Ac5O9krW9az3fLYFEZW4KoF1sr1hqDoVgrvv1zwc"));
            oauth1.setServerAuthURL("https://secure.sharefile.com/oauth/authorize");
            oauth1.setServerTokenURL("https://" + subdomain + ".sharefile.com/oauth/token");

            oauth1.setWebServerPort(7878); // Specify a particular port, as it will be used in the redirect.
            oauth1.startWebServer(); // In most cases, the webserver is automatically started when getAuthorization is called.  In this case, due to the use of oauth.nsoftware.com, it is started manually.
            oauth1.setReturnURL("https://oauth.nsoftware.com/oauthdemo"); // This URL should NOT be used in a production application.  It is only for testing purposes.
            oauth1.addParam("state", "http://localhost:" + oauth1.getWebServerPort()); // The oauth.nsoftware.com server uses this value to redirect to the local server embedded within the OAuth class.
            
			//oauth1.setRefreshToken(""); // Set to value from previous runs to expedite the login process.
            sharefile1.setAuthorization(oauth1.getAuthorization());
            //System.out.println(oauth1.getRefreshToken()); // RefreshToken may be used to automatically obtain a new token without user interaction.

            System.out.println("Home Directory Listings: ");
            listItems("home");
            printMenu();
            while (!buffer.equals("q")) {
                System.out.print("Enter command: ");
                buffer = input();
                switch (buffer){
                  case "ls":
                    listItems(currentFolderId);
                    break;
                  case "cd":
                    System.out.print("Number corresponding to folder you wish to view: ");
                    int folderSelection = Integer.valueOf(input());
                    if(folderSelection >= 0){
                      listItems(sharefile1.getItems().get(folderSelection).getId());
                    } else {
                      sharefile1.getItemInfo(currentFolderId);
                      listItems(sharefile1.getItems().get(0).getParentId());
                    }                 
                    break;
                  case "get":
                    System.out.print("Number corresponding to item you wish to download: ");
                    ShareFileItem downloadSelection = sharefile1.getItems().get(Integer.valueOf(input()));
                    System.out.print("Directory in which to download: ");

                    sharefile1.setLocalFile(input() + "\\" + downloadSelection.getName());
                    sharefile1.downloadFile(downloadSelection.getId()); 

                    System.out.println("\nDownload complete.\n");
                    break;
                  case "put":
                    System.out.print("Path of file that will be uploaded: ");
                    sharefile1.setLocalFile(input());

                    File mFile = new File(sharefile1.getLocalFile());
                    sharefile1.uploadFile(mFile.getName(), currentFolderId);

                    listItems(currentFolderId);
                    break;
                  case "del":
                    System.out.print("Number corresponding to item you wish to delete: ");
                    ShareFileItem deleteSelection = sharefile1.getItems().get(Integer.valueOf(input()));

                    sharefile1.deleteItem(deleteSelection.getId());

                    listItems(currentFolderId);
                    break;
                  case "mkdir":
                    System.out.print("Name of new folder: ");
                    sharefile1.createFolder(input(), currentFolderId);
                    listItems(currentFolderId);
                    break;
                  case "share":
                    System.out.print("Number corresponding to item you wish to share: ");
                    ShareFileItem shareSelection = sharefile1.getItems().get(Integer.valueOf(input()));
                    System.out.println("Share Link: " + sharefile1.createLink(shareSelection.getId()));
                    break;
                  case "request":
                    System.out.println("Request Link: " + sharefile1.createRequestLink(currentFolderId));
                    break;
                  case "q":
                    System.exit(0);
                    break;
                  default:
                    printMenu();
                } 
            }
        } catch (CloudStorageException e) {
            System.out.println(e.getMessage());
            System.exit(e.getCode());
            return;
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }

    }

    private void printMenu() {
        System.out.println("\n?\t-\tHelp\n" + "ls\t-\tList Items\n"
                + "cd\t-\tChange Folders\n" + "get\t-\tDownload Item\n" 
                + "put\t-\tUpload Item\n" + "del\t-\tDelete Item\n" 
                + "mkdir\t-\tCreate Folder\n" + "share\t-\tShare Items\n" 
                + "request\t-\tRequest Items\n" + "q\t-\tQuit\n");
    }

    private void listItems(String FolderId) {
        try {
            sharefile1.setAccountSubdomain(subdomain);
            sharefile1.getItemInfo(FolderId);
            currentFolderId = sharefile1.getItems().get(0).getId();

            System.out.print("\n");
            System.out.format("%1$-3s%2$-20s%3$-30s%4$-40s\n","#","Title","Type","Creation Date");
            System.out.println("--------------------------------------------------------------------------------");
            
            sharefile1.listItems(currentFolderId);
            System.out.format("%1$-3s%2$-20.18s%3$-30s%4$-40s\n", "-1", "..", "Folder", "");
            for (int i = 0; i < sharefile1.getItems().size(); i++) {
                ShareFileItem currentItem = sharefile1.getItems().get(i);
                System.out.format("%1$-3s%2$-20.18s%3$-30s%4$-40s\n", String.valueOf(i), currentItem.getName(),
                    ResolveItemType(currentItem.getType()), currentItem.getCreationDate());
       }

            System.out.print("\n");

        } catch (CloudStorageException e) {
            System.out.println(e.getMessage());
            System.exit(e.getCode());
            return;
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }

    private String ResolveItemType(int type) {
        switch (type) {
            case ShareFileItem.sfitFile:	return "File";
            case ShareFileItem.sfitFolder: return "Folder";
            default: return "Unknown";
        }
    }

    public static void main(String[] args) {
        try {
            new sharefile();
        } catch (Exception ex) {
            System.out.println("Exception: " + ex.getMessage());
        }
    }

}
class ConsoleDemo {
  private static BufferedReader bf = new BufferedReader(new InputStreamReader(System.in));

  static String input() {
    try {
      return bf.readLine();
    } catch (IOException ioe) {
      return "";
    }
  }
  static char read() {
    return input().charAt(0);
  }

  static String prompt(String label) {
    return prompt(label, ":");
  }
  static String prompt(String label, String punctuation) {
    System.out.print(label + punctuation + " ");
    return input();
  }

  static String prompt(String label, String punctuation, String defaultVal)
  {
	System.out.print(label + " [" + defaultVal + "] " + punctuation + " ");
	String response = input();
	if(response.equals(""))
		return defaultVal;
	else
		return response;
  }

  static char ask(String label) {
    return ask(label, "?");
  }
  static char ask(String label, String punctuation) {
    return ask(label, punctuation, "(y/n)");
  }
  static char ask(String label, String punctuation, String answers) {
    System.out.print(label + punctuation + " " + answers + " ");
    return Character.toLowerCase(read());
  }

  static void displayError(Exception e) {
    System.out.print("Error");
    if (e instanceof CloudStorageException) {
      System.out.print(" (" + ((CloudStorageException) e).getCode() + ")");
    }
    System.out.println(": " + e.getMessage());
    e.printStackTrace();
  }
}



