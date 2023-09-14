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

public class dropbox extends ConsoleDemo {
	Dropbox dropbox1 = new Dropbox();

	public dropbox() {
		String buffer = "";

		try {
			dropbox1.addDropboxEventListener(new cloudstorage.DefaultDropboxEventListener() {
						public void SSLServerAuthentication(
								DropboxSSLServerAuthenticationEvent e) {
							e.accept = true;
						}
					});

			System.out.println("********************************************************************************");
			System.out.println("This demo shows how to List, Upload, Download, and Delete documents from DropBox");
			System.out.println("To begin, you will first need to setup an app in the dropbox developer console  ");
			System.out.println("to obtain your App Key and App Secret. You will also need to setup a Redirect   ");
			System.out.println("URI to http://localhost:PORT, where PORT is the port number configured below.   ");
			System.out.println("********************************************************************************");
			
			dropbox1.getOAuth().setClientId(prompt("Client Id", ":"));
			dropbox1.getOAuth().setClientSecret(prompt("Client Secret", ":"));
			dropbox1.getOAuth().setServerAuthURL("https://www.dropbox.com/oauth2/authorize");
			dropbox1.getOAuth().setServerTokenURL("https://api.dropboxapi.com/oauth2/token");
			dropbox1.config("OAuthWebServerPort=" + Integer.parseInt(prompt("Redirect URI Port", ":")));
			dropbox1.authorize();
			
			printMenu();
			while (!buffer.equals("q")) {
				System.out.print("Enter command: ");
				buffer = input();
				if (buffer.toLowerCase().equals("ls")) {
					ListDocuments();
				} else if (buffer.toLowerCase().equals("get")) {
					System.out.print("Document #: ");
					DropboxResource r = dropbox1.getResources().get(Integer.valueOf(input()));
					System.out.print("Local Directory: ");
					
					dropbox1.setLocalFile(input() + "\\" + r.getName());
					dropbox1.downloadFile(r.getId()); //Use the default file format
					
					System.out.println("\nDownload Complete.\n");
				} else if (buffer.toLowerCase().equals("put")) {
					System.out.print("Local File: ");
					dropbox1.setLocalFile(input());
					File mFile = new File(dropbox1.getLocalFile());
					dropbox1.uploadFile(mFile.getName());
					
					ListDocuments();
				} else if (buffer.toLowerCase().equals("del")) {
					System.out.print("Document #: ");
					DropboxResource r = dropbox1.getResources().get(Integer.valueOf(input()));
					
					dropbox1.deleteResource(r.getId());
					
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
			dropbox1.listResources("");

			System.out.print("\n");
			System.out.format("%1$-3s%2$-20s%3$-30s%4$-40s\n","#","Title","Type","Last Modified");
			System.out.println("--------------------------------------------------------------------------------");
			
			for (int i = 0; i < dropbox1.getResources().size(); i++) {
				System.out.format("%1$-3s%2$-20.18s%3$-30s%4$-40s\n", String.valueOf(i), dropbox1.getResources().get(i).getName(),
					ResolveResourceType(dropbox1.getResources().get(i).getType()), dropbox1.getResources().get(i).getModifiedTime());
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
		case DropboxResource.drtFile:	return "File";
		case DropboxResource.drtFolder: return "Folder";
		default: return "Unknown";
		}
	}
	
	public static void main(String[] args) {
		try {
			new dropbox();
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



