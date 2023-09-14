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

public class wasabi  extends ConsoleDemo {
	
	public wasabi(){
		Wasabi wasabi1 = new Wasabi();
		String buffer= "";
		try{
			wasabi1.addWasabiEventListener(new WasabiEvents());
			System.out.print("Wasabi Access Key: ");
			wasabi1.setAccessKey(input());
			System.out.print("Wasabi Secret Key: ");
			wasabi1.setSecretKey(input());
			System.out.println("\nBucket List:\n");
			wasabi1.listBuckets();
			printMenu();
			while(!buffer.equals("q"))
			{
				System.out.print("Enter command: ");
				buffer = input();
				if(buffer.toLowerCase().equals("cd")){
					System.out.print("Which bucket?: ");
					buffer = input();
					wasabi1.setBucket(buffer);
				}
				else if(buffer.toLowerCase().equals("ls")){
					wasabi1.listObjects();
				}
				else if(buffer.toLowerCase().equals("get")){
					String rem_object;
					System.out.print("Name of object to download: ");
					rem_object = input();
					System.out.print("Directory to download to: ");
					wasabi1.setLocalFile(input() + "\\" + rem_object);
					wasabi1.getObject(rem_object);
				}
				else if(buffer.toLowerCase().equals("put")){
					String rem_object;
					System.out.print("Name of new object: ");
					rem_object = input();
					System.out.print("Local file to upload: ");
					wasabi1.setLocalFile(input());
					wasabi1.createObject(rem_object);
					System.out.println("Object created.");
				}
				else if(buffer.toLowerCase().equals("q")){
					System.exit(0);
				}	
				else if(buffer.toLowerCase().equals("?")){
					printMenu();
				}
			}
		}catch(CloudStorageException e)
        {
            System.out.println(e.getMessage());
            System.exit(e.getCode());
            return;
        }
        catch(Exception e)
        {
            System.out.println(e.getMessage());
        }
		
	}
	
   private void printMenu(){
    	System.out.println("\n?\t-\tHelp\n" +
				"cd\t-\tChange to bucket\n" +
				"ls\t-\tList Objects\n" +
				"get\t-\tGet remoteFile\n" +
				"put\t-\tPut localFile\n" +
				"q\t-\tQuit\n");
    }
    
	public static void main(String[] args) {
		try{
			new wasabi();
		}catch (Exception ex){
			System.out.println("Exception: "+ ex.getMessage());
		}
	}
	
}

class WasabiEvents extends DefaultWasabiEventListener {
	public void bucketList(WasabiBucketListEvent arg) {
		System.out.println(arg.bucketName);
	}
	public void objectList(WasabiObjectListEvent arg) {
		System.out.println(arg.objectName);
	}
	public void SSLServerAuthentication(WasabiSSLServerAuthenticationEvent arg) {
		arg.accept = true;
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



