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
#define LINE_LEN 150

class MyB2 : public BackblazeB2 {
public:
  virtual int FireFileList(BackblazeB2FileListEventParams* e) {
    printf("%s\n", e->Name);
    return 0;
  }

  virtual int FireBucketList(BackblazeB2BucketListEventParams* e) {
    printf("%s (%s)\n", e->Name, e->Id);
    return 0;
  }

  virtual int FireError(BackblazeB2ErrorEventParams* e) {
    printf("Error Code: %d, Description: %s", e->ErrorCode, e->Description);
    return 0;
  }
};

void printoptions() {
  printf("Type ? to see this menu, or a number 1-6 to perform one of the following actions:\n"
    "1. List Buckets\n"
    "2. Select Bucket\n"
    "3. List Files\n"
    "4. Download File\n"
    "5. Upload File\n"
    "6. Quit\n");
}

int main(int argc, char** argv) {
  MyB2 b2;
  char buffer[LINE_LEN + 1];
  char* bucketId = NULL;
  char* bucketName = NULL;
  int ret_code = 0;

  printf("Backblaze B2 Application Key Id: ");
  scanf("%150s", buffer);
  b2.SetApplicationKeyId(buffer);
  printf("Backblaze B2 Application Key: ");
  scanf("%150s", buffer);
  b2.SetApplicationKey(buffer);

  printoptions();
  while (1) {
    printf("b2> ");
    scanf("%150s", buffer);
    if (!strcmp(buffer, "?")) {
      printoptions();
    } else if (!strcmp(buffer, "1")) {
      if (ret_code = b2.ListBuckets()) goto error;
    } else if (!strcmp(buffer, "2")) {
bucket:
      printf("Enter Bucket Id: ");
      scanf("%150s", buffer);
      if (!strcmp(buffer, "")) goto bucket;
      if (bucketId != NULL) free((void*)bucketId);
      if (bucketName != NULL) free((void*)bucketName);
      bucketId = strdup(buffer);
      if (ret_code = b2.GetBucketInfo(bucketId)) goto error;
      bucketName = strdup(b2.GetBucketName(0));
    } else if (!strcmp(buffer, "3")) {
      if (ret_code = b2.ListFiles(bucketId)) goto error;
    } else if (!strcmp(buffer, "4")) {
file:
      printf("Which file: ");
      scanf("%150s", buffer);
      if (!strcmp(buffer, "")) goto file;
      b2.SetLocalFile("");
      if (ret_code = b2.DownloadFile(bucketName, buffer)) goto error;
      char* data; int size;
      if (ret_code = b2.GetFileData(data, size)) goto error;
      printf("Contents of file %s :\n", buffer);
      for (int i = 0; i < size; i++) {
        printf("%c", data[i]);
      }
      printf("\n");
    } else if (!strcmp(buffer, "5")) {
locfile:
      printf("Local file to upload: ");
      scanf("%150s", buffer);
      if (!strcmp(buffer, "")) goto locfile;
      b2.SetLocalFile(buffer);
remfile:
      printf("Name of new remote file: ");
      scanf("%150s", buffer);
      if (!strcmp(buffer, "")) goto remfile;
      char* newFileId = b2.UploadFile(bucketId, buffer);
      if (ret_code = b2.GetLastErrorCode()) goto error;
      printf("File %s created with Id %s.\n", buffer, newFileId);
    } else if (!strcmp(buffer, "6")) {
      exit(0);
    } else if (!strcmp(buffer, "")) {
      // Do nothing
    } else {
      printf("Please select a number from [1-6].\n");
    }
  }
  return ret_code;

error:
  printf("\nError: %d", ret_code);
  if (b2.GetLastError()) {
    printf(" \"%s\"\n", b2.GetLastError());
  }
  return ret_code;
}

