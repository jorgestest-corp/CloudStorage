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
#define LINE_LEN 100

class MyOneDrive : public OneDrive
{
public:
	
	virtual int FireSSLServerAuthentication(OneDriveSSLServerAuthenticationEventParams *e)
	{
		e->Accept = true;
		return 0;
	}
};

MyOneDrive onedrive1;

void printOptions()
{
	printf( "Type ? to select this menu, a number 1-5 to select the following choices:\n"
	        "1.  List Documents\n"
	        "2.  Download Document\n"
	        "3.  Upload Document\n"
	        "4.  Delete Document\n"
	        "5.  Quit\n");
}

char* resolveResourceType(int type) {
	switch (type)
	{
		case ODRT_FILE: return "File";
		case ODRT_FOLDER: return "Folder";
		default: return "Unknown";
	}
}

void displayDocuments(){
	onedrive1.SetRemoteId(""); // Clear
	onedrive1.SetRemotePath("");
	int ret_code = onedrive1.ListResources();
	if(ret_code)
		goto error;

	printf("%-3.1s%-40.38s%-10.8s%-25.23s\n\n","#","Title","Type","Modified Date");

	for(int i=0;i<onedrive1.GetResourceCount();i++)
	{
		printf("%-3.1d%-40.38s%-10.8s%-25.23s\n", i, onedrive1.GetResourceName(i), resolveResourceType(onedrive1.GetResourceType(i)), onedrive1.GetResourceModifiedTime(i));
	}
	return;

error:
	printf("\nError: %d", ret_code);
	if (onedrive1.GetLastError())
	{
		printf( " \"%s\"\n", onedrive1.GetLastError() );
	}
	printf("Press any key to continue");
	fflush(stdin);
	getchar();
	exit(ret_code);
}

int main(int argc, char **argv)
{	
  char buffer[LINE_LEN+1];
  int ret_code = 0;

  printf("***************************************************************\n");
  printf("* This demo shows how to use the OneDrive component to list,  *\n");
  printf("* download, upload, and delete documents.                     *\n");
  printf("***************************************************************\n\n");

  printf("Client ID: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer)-1] = '\0';
  onedrive1.SetOAuthClientId(buffer);
  
  printf("Client Secret: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer)-1] = '\0';
  onedrive1.SetOAuthClientSecret(buffer);

  onedrive1.SetOAuthServerAuthURL("https://login.microsoftonline.com/common/oauth2/v2.0/authorize");
  onedrive1.SetOAuthServerTokenURL("https://login.microsoftonline.com/common/oauth2/v2.0/token");
  onedrive1.Config("OAuthWebServerPort=7777"); // http://localhost:7777 is a registered redirect_url for the app
  onedrive1.SetOAuthAuthorizationScope("offline_access files.readwrite files.readwrite.all");

  printf("\nPerforming OAuth authentication ...\n");

  onedrive1.Authorize();

  ret_code = onedrive1.GetLastErrorCode();

  if(ret_code)
    goto error;

	printf("\n");
	printOptions();

	while (1)
	{
		printf( "\n> " );
		scanf("%80s",buffer);

		if ( ! strcmp(buffer, "?") )
		{
			printOptions();
		}

		else if ( ! strcmp(buffer, "1") ) //List documents
		{
			displayDocuments();
		}

		else if ( ! strcmp(buffer, "2") ) //Download document
		{
			printf("\nDocument Number: ");
			scanf("%80s",buffer);
			int docNum = atoi(buffer);
			
			printf("Download Directory: ");
			scanf("%80s",buffer);
			strcat(buffer,"\\");
			strcat(buffer,onedrive1.GetResourceName(docNum));
			onedrive1.SetLocalFile(buffer);
			onedrive1.SetRemotePath(onedrive1.GetResourcePath(docNum));

			ret_code = onedrive1.DownloadFile(); //Use the default file format
			if(ret_code)
				goto error;

			printf("\n Download Successful\n");
		}

		else if ( ! strcmp(buffer, "3") ) //Upload document
		{
			printf("\nLocal File: ");
			scanf("%80s",buffer);
			onedrive1.SetLocalFile(buffer);
			
			printf("Name of New File on Server: ");
			scanf("%80s",buffer);
            onedrive1.UploadFile(buffer);
			ret_code = onedrive1.GetLastErrorCode();

			if(ret_code)
				goto error;

			displayDocuments();
		}

		else if ( ! strcmp(buffer, "4") ) //Delete Document
		{
			printf("\nDocument Number: ");
			scanf("%80s",buffer);
			int docNum = atoi(buffer);

			onedrive1.SetRemoteId(onedrive1.GetResourceId(docNum));

			onedrive1.DeleteResource();
			ret_code = onedrive1.GetLastErrorCode();

			if(ret_code)
				goto error;

			displayDocuments();
		}

		else if ( ! strcmp(buffer, "5") )
		{
			exit(0);
		}

		else if ( ! strcmp(buffer, "") )
		{
			// Do nothing
		}
		else
		{
			printf( "Please select a number from [1-5].\n" );
		} // end of command checking
	}  // end of main while loop
	return ret_code;

error:
	printf("\nError: %d", ret_code);
	if (onedrive1.GetLastErrorCode())
	{
		printf( " \"%s\"\n", onedrive1.GetLastError() );
	}
	printf("Press any key to continue");
	fflush(stdin);
	getchar();
	return ret_code;

}


