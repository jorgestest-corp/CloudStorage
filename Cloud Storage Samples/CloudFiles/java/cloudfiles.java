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
import cloudstorage.*;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Scanner;

public class cloudfiles {

    public cloudfiles() {
        Scanner scanner = new Scanner(System.in);
        Cloudfiles cloudfiles = new Cloudfiles();
        Oauth oauth = new Oauth();

        try {
            cloudfiles.addCloudfilesEventListener(new CloudfilesEvents());

            System.out.println("Which cloud service provider would you like to use to send your message?");
            System.out.println("   [0] - Amazon S3");
            System.out.println("   [1] - GoogleStorage");
            System.out.println("   [2] - Azure Blob");

            int servicePrompt = Integer.parseInt(scanner.nextLine());

            // Prompt for authentication information.
            if (servicePrompt == 0) {
                cloudfiles.setServiceProvider(0);

                System.out.print("Enter your Amazon access key:  ");
                cloudfiles.getAccount().setAccessKey(scanner.nextLine());

                System.out.print("Enter your Amazon secret key:  ");
                cloudfiles.getAccount().setSecretKey(scanner.nextLine());
            } else if (servicePrompt == 1) {
                cloudfiles.setServiceProvider(9);
                oauth.setServerAuthURL("https://accounts.google.com/o/oauth2/auth");
                oauth.setServerTokenURL("https://accounts.google.com/o/oauth2/token");

                System.out.print("Enter your OAuth client ID:  ");
                oauth.setClientId(scanner.nextLine());

                System.out.print("Enter your OAuth client secret:  ");
                oauth.setClientSecret(scanner.nextLine());

                System.out.print("Authenticating... ");
                //cloudfiles.Account.Authorization = await oauth.GetAuthorization();
                cloudfiles.getAccount().setAuthMechanism(5);
                cloudfiles.setAuthorization(oauth.getAuthorization());
                System.out.print(" done.");
            } else if (servicePrompt == 2) {
                cloudfiles.setServiceProvider(6);
                oauth.setServerAuthURL("https://login.microsoftonline.com/common/oauth2/v2.0/authorize");
                oauth.setServerTokenURL("https://login.microsoftonline.com/common/oauth2/v2.0/token");

                System.out.print("Enter your OAuth client ID:  ");
                oauth.setClientId(scanner.nextLine());

                System.out.print("Enter your OAuth client secret:  ");
                oauth.setClientSecret(scanner.nextLine());

                System.out.print("Authenticating... ");
                cloudfiles.getAccount().setAuthMechanism(5);
                cloudfiles.setAuthorization(oauth.getAuthorization());
                System.out.print(" done.");
            } else {
                throw new Exception("\nInvalid cloud service provider.\n");
            }

            // Prompt for message information.
            System.out.println("What would you like to do?\n ");
            System.out.println("   [0] - Upload a file");
            System.out.println("   [1] - Download a file");
            System.out.println("   [2] - Delete a file");
            System.out.println("   [3] - Create Directory");
            System.out.println("   [4] - Delete Directory");
            System.out.println("   [5] - Quit");
            servicePrompt = Integer.parseInt(scanner.nextLine());

            String filename;
            String localFile;
            while (servicePrompt != 5) {
                switch (servicePrompt) {
                    case 0:
                        System.out.print("Remote file path: ");
                        filename = scanner.nextLine();
                        System.out.print("Local file path: ");
                        localFile = scanner.nextLine();
                        cloudfiles.setLocalFile(localFile);
                        cloudfiles.setRemoteFile(filename);
                        cloudfiles.upload();
                        System.out.println("Done.");
                        break;
                    case 1:
                        System.out.print("Remote file path: ");
                        filename = scanner.nextLine();
                        System.out.print("Local file path: ");
                        localFile = scanner.nextLine();
                        cloudfiles.setRemoteFile(filename);
                        cloudfiles.setLocalFile(localFile);
                        cloudfiles.download();
                        System.out.println("Done.");
                        break;
                    case 2:
                        System.out.print("File path: ");
                        filename = scanner.nextLine();
                        cloudfiles.deleteFile(filename);
                        System.out.println("Done.");
                        break;
                    case 3:
                        System.out.print("Directory name: ");
                        filename = scanner.next();
                        cloudfiles.makeDirectory(filename);
                        System.out.println("Done.");
                        break;
                    case 4:
                        System.out.print("Directory name: ");
                        filename = scanner.nextLine();
                        cloudfiles.removeDirectory(filename);
                        System.out.println("Done.");
                        break;
                    default:
                        throw new IllegalStateException("Unexpected value: " + servicePrompt);
                }
                System.out.println("What would you like to do?\n ");
                System.out.println("   [0] - Upload a file");
                System.out.println("   [1] - Download a file");
                System.out.println("   [2] - Delete a file");
                System.out.println("   [3] - Create Directory");
                System.out.println("   [4] - Delete Directory");
                System.out.println("   [5] - Quit");
                servicePrompt = Integer.parseInt(scanner.nextLine());
            }
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }

        System.out.println("\nPress any key to exit...");

    }
    public static void main(String[] args) {
        try {
            new cloudfiles();
        } catch (Exception e) {
            System.out.println("Exception: " + e.getMessage());
        }
    }


}

 class CloudfilesEvents extends DefaultCloudfilesEventListener {

    Scanner scn = new Scanner(System.in);

    public void SSLServerAuthentication(CloudfilesSSLServerAuthenticationEvent e) {
        if (e.accept) return;
        System.out.print("Server provided the following certificate:\nIssuer: " + e.certIssuer + "\nSubject: " + e.certSubject + "\n");
        System.out.print("The following problems have been determined for this certificate: " + e.status + "\n");
        System.out.print("Would you like to continue anyways? [y/n] ");
        if (scn.nextLine().charAt(0) == 'y') e.accept = true;
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



