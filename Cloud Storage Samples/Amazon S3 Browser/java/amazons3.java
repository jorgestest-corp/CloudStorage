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

public class amazons3  extends ConsoleDemo {
	
	public amazons3(){
		Amazons3 amazons31 = new Amazons3();
		String buffer= "";
		try{
			amazons31.addAmazons3EventListener(new S3Events());
			System.out.print("AWS Access Key: ");
			amazons31.setAccessKey(input());
			System.out.print("AWS Secret Key: ");
			amazons31.setSecretKey(input());
			System.out.println("\nBucket List:\n");
			amazons31.listBuckets();
			printMenu();
			while(!buffer.equals("q"))
			{
				System.out.print("Enter command: ");
				buffer = input();
				if(buffer.toLowerCase().equals("cd")){
					System.out.print("Which bucket?:");
					buffer = input();
					amazons31.setBucket(buffer);
				}
				else if(buffer.toLowerCase().equals("ls")){
					amazons31.listObjects();
				}
				else if(buffer.toLowerCase().equals("get")){
					System.out.print("Which file: ");
					amazons31.getObject(input());
					System.out.println("The content of the file:\n" + new String(amazons31.getObjectData()));
				}
				else if(buffer.toLowerCase().equals("put")){
					String rem_object;
					System.out.print("Name of new object:");
					rem_object = input();
					System.out.print("Local file to upload:");
					amazons31.setLocalFile(input());
					amazons31.createObject(rem_object);
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
			new amazons3();
		}catch (Exception ex){
			System.out.println("Exception: "+ ex.getMessage());
		}
	}
	
}

class S3Events extends DefaultAmazons3EventListener {
	public void bucketList(Amazons3BucketListEvent arg) {
		System.out.println(arg.bucketName);
	}
	public void objectList(Amazons3ObjectListEvent arg) {
		System.out.println(arg.objectName);
	}
	public void SSLServerAuthentication(Amazons3SSLServerAuthenticationEvent arg) {
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



