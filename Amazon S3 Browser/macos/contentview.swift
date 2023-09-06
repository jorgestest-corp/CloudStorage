import SwiftUI
import CloudStorage

struct Bucket: Identifiable {
  var id = UUID()
  let bucket: String
}
struct Objects: Identifiable {
  var id = UUID()
  let object: String
}
class BucketViewModel: ObservableObject {
  @Published var buckets = [Bucket]()
}
class ObjectViewModel: ObservableObject {
  @Published var objects = [Objects]()
}

struct ContentView: View, AmazonS3Delegate {
  func onBucketList(bucketName: String, creationDate: String, ownerId: String, ownerName: String) {
    let newBucket = Bucket(bucket: bucketName)
    bucketsViewModel.buckets.append(newBucket)
  }
  
  func onEndTransfer(direction: Int32) {}
  func onError(errorCode: Int32, description: String) {}
  func onFragmentComplete(fragmentNumber: Int32, fragmentCount: Int32, interrupt: inout Bool) {}
  func onHeader(field: String, value: String) {}
  func onLog(logLevel: Int32, message: String, logType: String) {}
  func onMetadataList(name: String, value: String) {}
  func onObjectList(bucketName: String, objectName: String, lastModified: String, size: Int64, eTag: String, ownerId: String, ownerName: String, uploadId: String, versionId: String, latestVersion: Bool, deleted: Bool) {
    let newBucket = Objects(object: objectName)
    objectsViewModel.objects.append(newBucket)
  }
  func onPartList(partNumber: Int32, objectName: String, lastModified: String, size: Int64, eTag: String, ownerId: String, ownerName: String) {}
  func onPrefixList(bucketName: String, prefix: String) {}
  func onProgress(direction: Int32, bytesTransferred: Int64, totalBytes: Int64, percentDone: Int32) {}
  func onSSLServerAuthentication(certEncoded: Data, certSubject: String, certIssuer: String, status: String, accept: inout Bool) {}
  func onSSLStatus(message: String) {}
  func onStartTransfer(direction: Int32) {}
  func onTransfer(direction: Int32, bytesTransferred: Int64, percentDone: Int32, text: Data) {}
  
  var s3 = AmazonS3()
  var documentsPath = NSSearchPathForDirectoriesInDomains(.documentDirectory, .userDomainMask, true)[0] + "/"
  @State private var secretkey: String = ""
  @State private var accesskey: String = ""
  @State private var buckets: String = ""
  @StateObject var bucketsViewModel = BucketViewModel()
  @StateObject var objectsViewModel = ObjectViewModel()
  
  var body: some View {
    VStack(alignment: .center)
    {
      Text("This demo shows how to use the S3 component to browse Simple Storage Service buckets and objects.")
        .padding(10)
        .foregroundColor(Color.blue)
      HStack {
        Text("Access Key:")
        TextField("enter access key...", text: $accesskey)
      }
      HStack {
        Text("Secret Key:")
        TextField("enter secret key...", text: $secretkey)
      }
      
      listBucketsButton()
      
      Text("Buckets:").bold().padding(.top, 5.0)
      
      List {
        ForEach(bucketsViewModel.buckets) { bucket in
          BucketRow(bucket: bucket.bucket)
            .onTapGesture {
              do {
                self.objectsViewModel.objects.removeAll()
                s3.bucket = bucket.bucket
                try s3.listObjects()
              } catch {
                print(error)
              }
            }
        }
      }
      
      Text("Tap/Click Bucket to List Objects:")
        .bold()
      List{
        ForEach(objectsViewModel.objects) { object in
          ObjectRow(object: object.object)
        }
      }
    }
    .padding(/*@START_MENU_TOKEN@*/.all, 8.0/*@END_MENU_TOKEN@*/)
    
  }
  
  @ViewBuilder
  private func listBucketsButton() -> some View {
    Button(action:
            {
      //client.runtimeLicense = ""
      s3.delegate = self
      do
      {
        s3.accessKey = accesskey
        s3.secretKey = secretkey
        try s3.listBuckets()
      }
      catch
      {
        print(error)
        return
      }
    }, label: {
      Text("List Buckets")
        .frame(minWidth: 140)
        .font(.system(size: 20))
        .background(RoundedRectangle(cornerRadius: 8)
          .fill(Color.gray))
    })
    .buttonStyle(PlainButtonStyle())
    
  }
  
}

struct BucketRow: View {
  let bucket: String
  var body: some View {
    Label(
      title: { Text(bucket)},
      icon: { Image(systemName: "circle.fill")}
    )
  }
}

struct ObjectRow: View {
  let object: String
  var body: some View {
    Label(
      title: { Text(object)},
      icon: { Image(systemName: "circle.fill")}
    )
  }
}

struct ContentView_Previews: PreviewProvider {
  static var previews: some View {
    ContentView()
  }
}
