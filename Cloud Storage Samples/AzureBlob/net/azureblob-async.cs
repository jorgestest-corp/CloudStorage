/*
 * Cloud Storage 2022 .NET Edition - Sample Project
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
 * 
 */

using System.Collections.Generic;
public class AzureblobDemo
{
  // Declare private static fields for AzureBlob blob and currentBucket string.
  private static Azureblob azureblob;
  private static string currentBucket;

  // Define Main function as an asynchronous task.
  static async Task Main(string[] args)
  {
    try
    {
      //Prompt the user to enter Azure Blob account and access key.
      Console.Write("Azure Account: ");
      var account = Console.ReadLine();

      Console.Write("Access Key: ");
      var accessKey = Console.ReadLine();

      // Initialize the AzureBlob object and set its account and access key.
      azureblob = new Azureblob();
      azureblob.Account = account;
      azureblob.AccessKey = accessKey;


      // Print container list title and then list all the containers.
      Console.WriteLine("\nContainer List:\n");
      await azureblob.ListContainers();
      foreach (var container in azureblob.Containers)
      {
        Console.WriteLine(container.Name);
      }

      // Print the menu options to the console.
      PrintMenu();

      // Loop until user enters "q" for quit.
      string buffer = "";
      while (!buffer.Equals("q", StringComparison.InvariantCultureIgnoreCase))
      {
        Console.Write("Enter command: ");
        buffer = Console.ReadLine();

        // Switch statement to handle different commands entered by the user.
        switch (buffer.ToLower().Trim())
        {
          case "cd":
            // Change the current container.
            Console.Write("Which container?: ");
            azureblob.Container = Console.ReadLine();
            Console.WriteLine($"Current container is now {azureblob.Container}.");
            break;

          case "ls":
            // List all blobs in the current container.
            try
            {
              await azureblob.ListBlobs();
            }
            catch (CloudStorageAzureblobException ex)
            {
              Console.WriteLine($"{ex.Message}");
              break;
            }
            foreach (var entry in azureblob.Blobs)
            {
              Console.WriteLine(entry.Name);
            }
            break;

          case "get":
            // Get an blob(file) from the current container.
            Console.Write("Which file: ");
            var blob = Console.ReadLine();
            await azureblob.GetBlob(blob);
            Console.WriteLine("The content of the file:\n=========================\n" + azureblob.BlobData + "\n=========================\n");
            break;

          case "put":
            // Upload a local file to the current container as a new blob.
            Console.Write("Name of new Blob: ");
            var blobName = Console.ReadLine();
            Console.Write("Local file to upload: ");
            azureblob.LocalFile = Console.ReadLine();
            await azureblob.CreateBlob(blobName, 0, -1);
            Console.WriteLine("Blob created.");
            break;

          case "q":
            // Exit the program.
            Environment.Exit(0);
            break;

          case "?":
            // Print the menu again.
            PrintMenu();
            break;

          default:
            Console.WriteLine("Invalid command!");
            break;
        }
      }
    }
    // Handle exceptions by printing the exception message to the console.
    catch (Exception e)
    {
      Console.WriteLine(e.Message);
    }
  }

  // Method to print the menu options to the console.
  private static void PrintMenu()
  {
    Console.WriteLine("\n?\t-\tHelp\n" +
        "cd\t-\tChange to Container\n" +
        "ls\t-\tList Blobs\n" +
        "get\t-\tView Blob Data\n" +
        "put\t-\tCreate Blob using local file\n" +
        "q\t-\tQuit\n");
  }
}


class ConsoleDemo
{
  public static Dictionary<string, string> ParseArgs(string[] args)
  {
    Dictionary<string, string> dict = new Dictionary<string, string>();

    for (int i = 0; i < args.Length; i++)
    {
      // If it starts with a "/" check the next argument.
      // If the next argument does NOT start with a "/" then this is paired, and the next argument is the value.
      // Otherwise, the next argument starts with a "/" and the current argument is a switch.

      // If it doesn't start with a "/" then it's not paired and we assume it's a standalone argument.

      if (args[i].StartsWith("/"))
      {
        // Either a paired argument or a switch.
        if (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
        {
          // Paired argument.
          dict.Add(args[i].TrimStart('/'), args[i + 1]);
          // Skip the value in the next iteration.
          i++;
        }
        else
        {
          // Switch, no value.
          dict.Add(args[i].TrimStart('/'), "");
        }
      }
      else
      {
        // Standalone argument. The argument is the value, use the index as a key.
        dict.Add(i.ToString(), args[i]);
      }
    }
    return dict;
  }

  public static string Prompt(string prompt, string defaultVal)
  {
    Console.Write(prompt + (defaultVal.Length > 0 ? " [" + defaultVal + "]": "") + ": ");
    string val = Console.ReadLine();
    if (val.Length == 0) val = defaultVal;
    return val;
  }
}