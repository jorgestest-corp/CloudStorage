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

public class onedrive extends ConsoleDemo {
    Onedrive onedrive1 = new Onedrive();

    public onedrive() {
        String buffer = "";

        try {
            onedrive1.addOnedriveEventListener(new cloudstorage.DefaultOnedriveEventListener() {
                public void SSLServerAuthentication(
                        OnedriveSSLServerAuthenticationEvent e) {
                    e.accept = true;
                }
            });

            onedrive1.getOAuth().setClientId(prompt("OAuth Client Id", ":"));
            onedrive1.getOAuth().setClientSecret(prompt("OAuth Client Secret", ":"));
            onedrive1.getOAuth().setServerAuthURL("https://login.microsoftonline.com/common/oauth2/v2.0/authorize");
            onedrive1.getOAuth().setServerTokenURL("https://login.microsoftonline.com/common/oauth2/v2.0/token");
            onedrive1.config("OAuthWebServerPort=7777"); // http://localhost:7777 is a registered redirect_url for the app
            onedrive1.getOAuth().setAuthorizationScope("offline_access files.readwrite files.readwrite.all");
            onedrive1.authorize();

            printMenu();
            while (!buffer.equals("q")) {
                System.out.print("Enter command: ");
                buffer = input();
                if (buffer.toLowerCase().equals("ls")) {
                    ListDocuments();
                } else if (buffer.toLowerCase().equals("get")) {
                    System.out.print("Document #: ");
                    OneDriveResource r = onedrive1.getResources().get(Integer.valueOf(input()));
                    System.out.print("Local Directory: ");

                    onedrive1.setLocalFile(input() + "\\" + r.getName());
                    onedrive1.setRemoteId(r.getId());
                    onedrive1.downloadFile(); //Use the default file format

                    System.out.println("\nDownload Complete.\n");
                } else if (buffer.toLowerCase().equals("put")) {
                    System.out.print("Local File: ");
                    onedrive1.setLocalFile(input());
                    onedrive1.setRemoteId("");
                    File mFile = new File(onedrive1.getLocalFile());
                    onedrive1.uploadFile(mFile.getName());

                    ListDocuments();
                } else if (buffer.toLowerCase().equals("del")) {
                    System.out.print("Document #: ");
                    OneDriveResource r = onedrive1.getResources().get(Integer.valueOf(input()));

                    onedrive1.setRemoteId(r.getId());
                    onedrive1.deleteResource();

                    ListDocuments();
                } else if (buffer.toLowerCase().equals("q")) {
                    System.exit(0);
                } else if (buffer.toLowerCase().equals("?")) {
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
        System.out.println("\n?\t-\tHelp\n" + "ls\t-\tList Documents\n"
                + "get\t-\tDownload Document\n" + "put\t-\tUpload Document\n"
                + "del\t-\tDelete Document\n" + "q\t-\tQuit\n");
    }

    private void ListDocuments() {
        try {
            onedrive1.setRemoteId(""); // Clear the documents collection
            onedrive1.setRemotePath("");
            onedrive1.listResources();

            System.out.print("\n");
            System.out.format("%1$-3s%2$-20s%3$-30s%4$-40s\n","#","Title","Type","Last Modified");
            System.out.println("--------------------------------------------------------------------------------");

            for (int i = 0; i < onedrive1.getResources().size(); i++) {
                System.out.format("%1$-3s%2$-20.18s%3$-30s%4$-40s\n", String.valueOf(i), onedrive1.getResources().get(i).getName(),
                    ResolveResourceType(onedrive1.getResources().get(i).getType()), onedrive1.getResources().get(i).getModifiedTime());
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

    private String ResolveResourceType(int type) {
        switch (type) {
            case OneDriveResource.odrtFile:	return "File";
            case OneDriveResource.odrtFolder: return "Folder";
            default: return "Unknown";
        }
    }

    public static void main(String[] args) {
        try {
            new onedrive();
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



