open System.IO
open Amazon
open Amazon.S3
open Amazon.S3.Transfer

[<EntryPoint>]
let main _ =
    let bucket = "apex-web"

    let currentDir = Directory.GetCurrentDirectory()
    let root = Path.Combine(currentDir, "..", "..", "..", "..", "..")
    let root = Path.GetFullPath(root)
    let root = Path.Combine(root, "web/dist/prod")

    let paths = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)

    let client = new AmazonS3Client(RegionEndpoint.USEast1)

    let tu = new TransferUtility(client)

    let prefix = root + (string Path.DirectorySeparatorChar)

    for p in paths do
        let key = p.Replace(prefix, "")
                   .Replace("\\", "/") //S3 only delimits paths on /, not \
        printfn "Uploading %s" key
        tu.Upload(p, bucket, key)

    printfn "Finished upload"
    0 //