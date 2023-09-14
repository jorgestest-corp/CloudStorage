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

class MyBox : public Box
{
public:
	
	virtual int FireSSLServerAuthentication(BoxSSLServerAuthenticationEventParams *e)
	{
		e->Accept = true;
		return 0;
	}
};

MyBox box1;

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
	case BRT_FILE: return "File";
	case BRT_FOLDER: return "Folder";
	default: return "Unknown";
	}
}


void displayDocuments(){

	int ret_code = box1.ListResources("");
	if(ret_code)
		goto error;

	printf("%-4.1s%-42.40s%-22.20s%\n\n","#","Title","Type");

	for(int i=0;i<box1.GetResourceCount();i++)
	{
		printf("%-4.1d%-42.40s%-22.20s%\n",i,box1.GetResourceName(i),resolveResourceType(box1.GetResourceType(i)));
	}
	return;

error:
	printf("\nError: %d", ret_code);
	if (box1.GetLastError())
	{
		printf( " \"%s\"\n", box1.GetLastError() );
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
  printf("* This demo shows how to use the Box component to list,       *\n");
  printf("* download, upload, and delete documents. To begin, you will  *\n");
  printf("* first need to setup an app in the box developer console     *\n");
  printf("* to obtain your App Key and App Secret. You will also need   *\n");
  printf("* to setup a Redirect URI to http://localhost:PORT, where     *\n");
  printf("* PORT is the port number configured in the code.             *\n");
  printf("***************************************************************\n\n");

  printf("Client ID: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer)-1] = '\0';
  box1.SetOAuthClientId(buffer);
  
  
  printf("Client Secret: ");
  fgets(buffer, LINE_LEN, stdin);
  buffer[strlen(buffer)-1] = '\0';
  box1.SetOAuthClientSecret(buffer);
 
  
  box1.SetOAuthServerAuthURL("https://app.box.com/api/oauth2/authorize");
  box1.SetOAuthServerTokenURL("https://api.box.com/oauth2/token");
  box1.Config("OAuthWebServerPort=7777");

  printf("\nPerforming OAuth authentication ...\n");

  box1.Authorize();

  ret_code = box1.GetLastErrorCode();

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
			strcat(buffer,box1.GetResourceName(docNum));
			box1.SetLocalFile(buffer);

			ret_code = box1.DownloadFile(box1.GetResourceId(docNum)); //Use the default file format
			if(ret_code)
				goto error;

			printf("\n Download Successful\n");
		}

		else if ( ! strcmp(buffer, "3") ) //Upload document
		{
			printf("\nLocal File: ");
			scanf("%80s",buffer);

			box1.SetLocalFile(buffer);
			
			printf("Name of New File on Server: ");
			scanf("%80s",buffer);
      		box1.UploadFile(buffer,"");
			ret_code = box1.GetLastErrorCode();

			if(ret_code)
				goto error;

			displayDocuments();
		}

		else if ( ! strcmp(buffer, "4") ) //Delete Document
		{
			printf("\nDocument Number: ");
			scanf("%80s",buffer);

			ret_code = box1.DeleteResource(box1.GetResourceId(atoi(buffer)));

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
	if (box1.GetLastErrorCode())
	{
		printf( " \"%s\"\n", box1.GetLastError() );
	}
	printf("Press any key to continue");
	fflush(stdin);
	getchar();
	return ret_code;

}

