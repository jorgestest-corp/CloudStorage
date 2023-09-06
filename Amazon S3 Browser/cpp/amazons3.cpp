/*
 * Cloud Storage 2022 C++ Edition - Sample Project
 *
 * This sample project demonstrates the usage of Cloud Storage in a 
 * simple, straightforward way. This is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * Copyright (c) 2023 /n software inc. www.nsoftware.com
 */

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "../../include/cloudstorage.h"
#define LINE_LEN 100



class MyS3 : public AmazonS3
{
public:

	virtual int FireObjectList(AmazonS3ObjectListEventParams *e)
	{
		printf("%s\n", e->ObjectName);
		return 0;
	}

	virtual int FireBucketList(AmazonS3BucketListEventParams *e)
	{
		printf("%s\n", e->BucketName);
		return 0;
	}

	virtual int FireError(AmazonS3ErrorEventParams *e)
	{
		printf("Error Code: %d, Description: %s",e->ErrorCode,e->Description);
		return 0;
	}

};

void printoptions()
{
	printf( "Type ? to select this menu, a number 1-5 to select the following choices:\n"
	        "1.  List Objects\n"
	        "2.  Get Object\n"
	        "3.  Get Object Link\n"
	        "4.  Put Object\n"
	        "5.  Quit\n");
}


int main(int argc, char **argv)
{

	MyS3 s3;
	char buffer[LINE_LEN+1];
	int ret_code = 0;
	int size = 0;

	printf("AWS Access Key: ");
	scanf("%80s",buffer);
	s3.SetAccessKey(buffer);
	printf("AWS Secret Key: ");
	scanf("%80s",buffer);
	s3.SetSecretKey(buffer);

	printf("Available buckets:\n");
	if( ret_code = s3.ListBuckets() ) goto error;

	printf("\nS3 Bucket to Examine: ");
	scanf("%80s",buffer);
	s3.SetBucket(buffer);

	printoptions();
	while (1)
	{
		printf( "s3> " );
		scanf("%80s",buffer);

		if ( ! strcmp(buffer, "?") )
		{
			printoptions();
		}

		else if ( ! strcmp(buffer, "1") )
		{
			if( ret_code = s3.ListObjects() ) goto error;
		}

		else if ( ! strcmp(buffer, "2") )
		{
			printf("Which object?: ");
			scanf("%80s",buffer);
			if ( ret_code = s3.GetObject(buffer) ) goto error;
			char * data;
			if ( ret_code = s3.GetObjectData(data, size) ) goto error;
			printf("Contents of object %s :\n",buffer);
			for(int i=0; i < size; i++)
			{
				printf("%c",data[i]);
			}
			printf("\n");
		}

		else if ( ! strcmp(buffer, "3") )
		{
			printf("Which object?: ");
			scanf("%80s",buffer);
			printf("%s\n", s3.GetLink(buffer, 60));
		}

		else if ( ! strcmp(buffer, "4") )
		{
			char objname[81];
			printf("Name of new object: ");
			scanf("%80s",objname);
			printf("Local file to upload: ");
			scanf("%80s",buffer);
			s3.SetLocalFile(buffer);
			if( ret_code = s3.CreateObject(objname) ) goto error;
			printf("Object %s created.\n",objname);
			s3.SetLocalFile("");
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
	if (s3.GetLastError())
	{
		printf( " \"%s\"\n", s3.GetLastError() );
	}
	return ret_code;

}


