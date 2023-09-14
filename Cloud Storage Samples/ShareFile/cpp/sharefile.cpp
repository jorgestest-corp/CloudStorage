/*
 * Cloud Storage 2022 C++ Edition - Sample Project
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

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "../../include/cloudstorage.h"
#include <string>
#define LINE_LEN 255

OAuth oauth1;
ShareFile sharefile1;
enum { IT_FILE, IT_FOLDER }; // Enums defining the item type.
char currentFolderId[LINE_LEN + 1]; // The ID of the folder that the user is currently in.

/*
* Method to print the error codes
*/
void print_error(int ret_code) {
	printf("\nError: %d", ret_code);
	if (sharefile1.GetLastError())
	{
		printf(" \"%s\"\n", sharefile1.GetLastError());
	}
	if (oauth1.GetLastError()) {
		printf(" \"%s\"\n", oauth1.GetLastError());
	}
	printf("Press any key to continue.");
	fflush(stdin);
	getchar();
	exit(ret_code);
}

/*
* Method to print the menu options
*/
void printOptions()
{
	printf( "Type ? to view this menu, or type one of the numbers below to perform the corresponding operation:\n"
	        "1.  List Items\n"
	        "2.  Change Folders\n"
			"3.  Download Items\n"
	        "4.  Upload Items\n"
	        "5.  Delete Items\n"
			"6.  Create Folder\n"
			"7.  Create Request Link\n"
			"8.  Create Share Link\n"
	        "9.  Quit\n");
}

/*
* Method that converts the item type enum to a readable string.
*/
char* resolveItemType(int type) {
  switch (type)
  {
    case IT_FILE: return "File";
    case IT_FOLDER: return "Folder";
    default: return "Unknown";
  }
}

/*
* Method that lists the items in the passed folder.  Updates currentFolderId.
*/
void displayItems(const char* folderId){

	sharefile1.GetItemInfo(folderId);
	strcpy(currentFolderId, sharefile1.GetItemId(0));
	int ret_code = sharefile1.ListItems(currentFolderId);
	if(ret_code)
		print_error(ret_code);

	printf("%-3.1s%-40.38s%-10.8s%-25.23s\n\n","#","Title","Type","Creation Date");
	
	for(int i=0;i<sharefile1.GetItemCount();i++)
	{
		printf("%-3.1d%-40.38s%-10.8s%-25.23s\n",i, sharefile1.GetItemName(i),resolveItemType(sharefile1.GetItemType(i)),sharefile1.GetItemCreationDate(i));
	}
	return;
}

/*
* Main Method
*/
int main(int argc, char **argv)
{
	char subdomain[LINE_LEN + 1] = "";
	char buffer[LINE_LEN + 1] = "";
	int ret_code = 0;

  printf("\n*****************************************************************");
  printf("\n* This demo shows how to use the ShareFile component to list,   *");
  printf("\n* upload, download, delete, and share documents from ShareFile. *");
  printf("\n* Default prompt values are in [ ] where applicable.            *");
  printf("\n*****************************************************************\n");

  printf("Account  Subdomain: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer) - 1] = '\0';
  if (strlen(buffer) != 0) {
	  strcpy(subdomain, buffer);
  } else {
	  printf("Must set a subdomain (press any key to quit): ");
	  fgets(buffer, LINE_LEN, stdin);
	  return -1;
  }

  printf("Client ID [Hb7kR4d6mBvQxnOe9Eg6V8CMIVjQ67vD]: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer)-1] = '\0';
  if (strlen(buffer) == 0) { 
    oauth1.SetClientId("Hb7kR4d6mBvQxnOe9Eg6V8CMIVjQ67vD");
  } else { 
    oauth1.SetClientId(buffer);
  }
  
  printf("Client Secret [jHAZH6f1Ac5O9krW9az3fLYFEZW4KoF1sr1hqDoVgrvv1zwc]: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer)-1] = '\0';
  if (strlen(buffer) == 0) {
    oauth1.SetClientSecret("jHAZH6f1Ac5O9krW9az3fLYFEZW4KoF1sr1hqDoVgrvv1zwc");
  } else {
    oauth1.SetClientSecret(buffer);
  }
  
  oauth1.SetClientProfile(0); // Application
  oauth1.SetServerAuthURL("https://secure.sharefile.com/oauth/authorize");
  
	//Build the ServerTokenURL string with the specified subdomain
	strcpy(buffer, "https://");
	strcat(buffer, subdomain);
    strcat(buffer, ".sharefile.com/oauth/token");
	buffer[strlen(buffer)] = '\0';
    oauth1.SetServerTokenURL(buffer);
	
	//Web Server (port in param must equal port in oauth1.SetWebServerPort())
	char* param = "http://localhost:7878";
	oauth1.SetWebServerPort(7878); // Specify a particular port, as it will be used in the redirect.
	oauth1.StartWebServer(); // In most cases, the webserver is automatically started when getAuthorization is called.  In this case, due to the use of oauth.nsoftware.com, it is started manually.
	oauth1.SetReturnURL("https://oauth.nsoftware.com/oauthdemo"); // This URL should NOT be used in a production application.  It is only for testing purposes.
	oauth1.AddParam("state", param); // The oauth.nsoftware.com server uses this value to redirect to the local server embedded within the OAuth class.
	//oauth1.SetRefreshToken(""); // Set to value from previous runs to expedite the login process.
    sharefile1.SetAuthorization(oauth1.GetAuthorization());
	//printf("Refresh Token: %s", oauth1.GetRefreshToken()); // RefreshToken may be used to automatically obtain a new token without user interaction.

	ret_code = oauth1.GetLastErrorCode();

	if (ret_code) {
		print_error(ret_code);
	}
 
	sharefile1.SetAccountSubdomain(subdomain);
    printf("\n");
	printf("Home Directory Listings: \n");
	displayItems("home");

	printOptions();

	while (1)
	{
		printf( "\n> " );
		scanf("%80s",buffer);

		if (!strcmp(buffer, "?") ) // List options.
		{
			printOptions();
		}

		else if (!strcmp(buffer, "1") ) // List items.
		{
			displayItems("home");
		}
		
		else if (!strcmp(buffer, "2")) // Change folders.
		{
			printf("\nNumber corresponding to folder you wish to view: ");
			scanf("%80s", buffer);
			int index = atoi(buffer);
			if (index < 0) {
				sharefile1.GetItemInfo(currentFolderId);
				strcpy(buffer, sharefile1.GetItemParentId(0));
				displayItems(buffer);
			}
			else {
				if (sharefile1.GetItemType(index) == IT_FOLDER) {
					strcpy(buffer, sharefile1.GetItemId(index));
					displayItems(buffer);
				}
				else {
					printf("\nPlease choose a valid directory.\n");
				}
			}
		}
		
		else if (!strcmp(buffer, "3") ) // Download item.
		{
			printf("\nNumber corresponding to item you wish to download: ");
			scanf("%80s",buffer);
			int index = atoi(buffer);
			
			printf("Directory in which to download: ");
			scanf("%80s",buffer);
			strcat(buffer,"\\");
			strcat(buffer,sharefile1.GetItemName(index));
			sharefile1.SetLocalFile(buffer);

			ret_code = sharefile1.DownloadFile(sharefile1.GetItemId(index));
			if(ret_code)
				print_error(ret_code);

			printf("\nDownload complete.\n");
		}

		else if (!strcmp(buffer, "4") ) // Upload file.
		{
			printf("\nPath of file that will be uploaded: ");
			scanf("%80s",buffer);

			sharefile1.SetLocalFile(buffer);
			
      sharefile1.UploadFile(sharefile1.GetLocalFile(), currentFolderId);
			ret_code = sharefile1.GetLastErrorCode();

			if(ret_code)
				print_error(ret_code);

			displayItems(currentFolderId);
		}

		else if (!strcmp(buffer, "5") ) // Delete item.
		{
			printf("\nNumber corresponding to item you wish to delete: ");
			scanf("%80s",buffer);
			int index = atoi(buffer);
			ret_code = sharefile1.DeleteItem(sharefile1.GetItemId(index));
			if(ret_code)
				print_error(ret_code);

			displayItems(currentFolderId);
		}

		else if (!strcmp(buffer, "6")) // Create folder.
		{
			printf("\nName of new folder: ");
			scanf("%80s", buffer);
			sharefile1.CreateFolder(buffer, currentFolderId);
			displayItems(currentFolderId);
		}

		else if (!strcmp(buffer, "7")) // Create request link.
		{
			printf("Request Link: %s\n", sharefile1.CreateRequestLink(currentFolderId));

		}

		else if (!strcmp(buffer, "8")) // Create share link.
		{
		printf("\nNumber corresponding to item you wish to share: ");
		scanf("%80s", buffer);
		int index = atoi(buffer);
		printf("Share Link: %s\n", sharefile1.CreateLink(sharefile1.GetItemId(index)));
		}

		else if (!strcmp(buffer, "9") )
		{
			exit(0);
		}

		else if (!strcmp(buffer, "") )
		{
			// Do nothing.
		}
		else
		{
			printf("\nPlease select a number from 1-9 or ? for possible commands.\n");
		}
	}
	return ret_code;
}

