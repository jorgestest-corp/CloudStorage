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

public class googledrive extends ConsoleDemo {
	Googledrive googledrive1 = new Googledrive();

	public googledrive() {
		String buffer = "";

		try {
			googledrive1.addGoogledriveEventListener(new cloudstorage.DefaultGoogledriveEventListener() {
						public void SSLServerAuthentication(
								GoogledriveSSLServerAuthenticationEvent e) {
							e.accept = true;
						}
					});

      googledrive1.getOAuth().setClientId(prompt("OAuth Client Id",":"));
      googledrive1.getOAuth().setClientSecret(prompt("OAuth Client Secret",":"));
			googledrive1.getOAuth().setServerAuthURL("https://accounts.google.com/o/oauth2/auth");
			googledrive1.getOAuth().setServerTokenURL("https://accounts.google.com/o/oauth2/token");
			googledrive1.getOAuth().setAuthorizationScope("https://www.googleapis.com/auth/drive");
			googledrive1.authorize();
			
			printMenu();
			while (!buffer.equals("q")) {
				System.out.print("Enter command: ");
				buffer = input();
				if (buffer.toLowerCase().equals("ls")) {
					ListDocuments();
				} else if (buffer.toLowerCase().equals("get")) {
					System.out.print("Document #: ");
					GDriveResource r = googledrive1.getResources().get(Integer.valueOf(input()));
					System.out.print("Local Directory: ");
					
					googledrive1.setLocalFile(input() + "\\" + r.getName());
					googledrive1.downloadFile(r.getId(), ""); //Use the default file format
					
					System.out.println("\nDownload Complete.\n");
				} else if (buffer.toLowerCase().equals("put")) {
					System.out.print("Local File: ");
					googledrive1.setLocalFile(input());
					googledrive1.getResources().clear();
					File mFile = new File(googledrive1.getLocalFile());
					googledrive1.uploadFile(mFile.getName(), "");
					
					ListDocuments();
				} else if (buffer.toLowerCase().equals("del")) {
					System.out.print("Document #: ");
					GDriveResource r = googledrive1.getResources().get(Integer.valueOf(input()));
					
					googledrive1.deleteResource(r.getId());
					
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
			googledrive1.listResources();

			System.out.format("%1$-3s%2$-20s%3$-30s%4$-40s\n","#","Name","Author","Last Modified");
			System.out.println("--------------------------------------------------------------------------------");
			
			for (int i = 0; i < googledrive1.getResources().size(); i++) {
				System.out.format("%1$-3s%2$-20.18s%3$-30s%4$-40s\n", String.valueOf(i), googledrive1.getResources().get(i).getName(),
					googledrive1.getResources().get(i).getOwner(), googledrive1.getResources().get(i).getModifiedTime());
			}
			
			System.out.println("\n\n");
			
		} catch (CloudStorageException e) {
			System.out.println(e.getMessage());
			System.exit(e.getCode());
			return;
		} catch (Exception e) {
			System.out.println(e.getMessage());
		}
	}

	public static void main(String[] args) {
		try {
			new googledrive();
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



