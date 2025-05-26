using common.lib.Configs;
using Azure.Identity;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.EventHubs;
using System.Text;

namespace eventhub.messagesender;

internal class Program
{
    private const string EventHubName = "dummypreview"; //"dummypreview" "dummyvideo";//"largevideogenerationrequired"; //"regeneratelargevideo"; 

    static async Task Main(string[] args)
    {
        IConfigurationRoot config = ConfigLoader.LoadConfiguration(new ConfigurationBuilder());

        string eventhubNamespace = config.GetSection("EventHubNamespaceName-1:fullyQualifiedNamespace").Value;


        EventHubProducerClient producerClient = new EventHubProducerClient(
            $"{eventhubNamespace}",
            EventHubName,
            new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                TenantId = config.GetSection("AadTenantId").Value
            }));


        List<string> messages = GetMessagesAv();

        Console.WriteLine($"Trying to send {messages.Count} messages to {EventHubName}...");

        try
        {
            foreach (string message in messages)
            {
                Console.WriteLine($"Sending {message} ...");
                using (EventDataBatch eventBatch = await producerClient.CreateBatchAsync())
                {
                    eventBatch.TryAdd(new EventData()
                    {
                        EventBody = new BinaryData(Encoding.UTF8.GetBytes(message))
                    });

                    await producerClient.SendAsync(eventBatch);
                }
            }
        }
        finally
        {
            await producerClient.CloseAsync();
            await producerClient.DisposeAsync();
        }

        Console.WriteLine($"Send {messages.Count} messages to {EventHubName}.");
    }

    private static List<string> GetMessagesAv()
    {
        return new List<string>()
        {
            """
               {
                   "asset": {
                       "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_4b4b217843881c6a7f4b4ba142b647aca8b9ef99ccb98ee67c73042ab322e90e",
                       "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
                       "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
                       "InternalId":"aa01ca1a-3bd0-4318-8ba9-4f4d4f7592de",
                       "TenantId":null,
                       "FolderId":null,
                       "Renditions":null,
                       "DisplayFilename":"sample_640x360.mp4",
                       "FileExtension":"mp4",
                       "FileType":"MPEG",
                       "AssetType":0,
                       "CreatedTime":"2024-04-02T13:17:37.6168985Z",
                       "FileTime":"2024-04-02T13:17:37.6169405Z",
                       "DeleteTime":null,
                       "FileSize":561,
                       "PageCount":120,
                       "AdditionalData":{},
                       "uploadedWithSidecar":true,
                       "indexTimeUtc":null,
                       "isLargeFile":false
                   }
               }
            """
            ,
            """
                {
                    "asset":
                        {
                            "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_72a596c8d0e424860c3c6af8274b27abbab62997c18e1582b5b5999cb0c3727b",
                            "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
                            "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
                            "InternalId":"9df1cc04-a036-4868-8ed4-1a6ab1ed4204",
                            "TenantId":null,
                            "FolderId":null,
                            "Renditions":null,
                            "DisplayFilename":"Concession_LAN_800k.mp4",
                            "FileExtension":"mp4",
                            "FileType":"MPEG",
                            "AssetType":0,
                            "CreatedTime":"2024-04-10T09:22:54.1867608Z",
                            "FileTime":"2024-04-10T09:22:54.186801Z",
                            "DeleteTime":null,
                            "FileSize":2461,
                            "PageCount":120,
                            "AdditionalData":{},
                            "uploadedWithSidecar":true,
                            "indexTimeUtc":null,
                            "isLargeFile":false
                          }
                 }
            """
            ,
            """
                {
                    "asset":
                        {
                            "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_9f1c91d14b330883a4800a06f850b660baaa4c2a465cf4ae47c951baac4ae17c",
                            "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
                            "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
                            "InternalId":"fb936925-0821-49fe-9038-2d3a8f1089cd",
                            "TenantId":null,
                            "FolderId":null,
                            "Renditions":null,
                            "DisplayFilename":"beach.mp4",
                            "FileExtension":"mp4",
                            "FileType":"MPEG",
                            "AssetType":0,
                            "CreatedTime":"2024-04-18T08:06:06.1744486Z",
                            "FileTime":"2024-04-18T08:06:06.1744928Z",
                            "DeleteTime":null,
                            "FileSize":16034,
                            "PageCount":120,
                            "AdditionalData":{},
                            "uploadedWithSidecar":true,
                            "indexTimeUtc":null,
                            "isLargeFile":false
                        }
                }
            """
            ,
            """
                {
                    "asset":
                        {
                            "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_806eaab40baf9889542e440bd58fd370e2ba39040d0c7e358334d115a102b3fc",
                            "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
                            "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
                            "InternalId":"7edb7e04-267b-44bd-9e6d-9b27c79c7cb9",
                            "TenantId":null,
                            "FolderId":null,
                            "Renditions":null,
                            "DisplayFilename":"friendseating (2).webm",
                            "FileExtension":"webm",
                            "FileType":"Unknown",
                            "AssetType":5,
                            "CreatedTime":"2024-04-18T08:12:33.0385028Z",
                            "FileTime":"2024-04-18T08:12:33.0385432Z",
                            "DeleteTime":null,
                            "FileSize":2278,
                            "PageCount":120,
                            "AdditionalData":{},
                            "uploadedWithSidecar":true,
                            "indexTimeUtc":null,
                            "isLargeFile":false
                        }
                }
            """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_a74017d5f6e8abee816b39ebfa28f807b2445137ee34bf044e314d335c792f6d",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"79f6b205-ea81-4d54-89b4-4e87574b09a3",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"babyandmother.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T08:17:02.1262794Z",
            //                 "FileTime":"2024-04-18T08:17:02.1263251Z",
            //                 "DeleteTime":null,
            //                 "FileSize":14870,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_0fbb00c185295a680094e15c3ee8bc3436644d0df6802925198bb3dde80c753c",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"8f2ce53f-31d4-4bcb-8213-05723f0bf63c",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"basketball.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T08:23:10.0294084Z",
            //                 "FileTime":"2024-04-18T08:23:10.02945Z",
            //                 "DeleteTime":null,
            //                 "FileSize":45926,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":false,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //                 }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_e25d2ee78e8d3ca57cc7191e24563910359d121feca3a8be1fbf5caf785b49f1",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"ea85dfa4-d1c5-4c4e-8b76-a7198fea74ce",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"bicycle.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T08:25:52.6313436Z",
            //                 "FileTime":"2024-04-18T08:25:52.6313457Z",
            //                 "DeleteTime":null,
            //                 "FileSize":12950,
            //                 "PageCount":60,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_b58766914a56c08ca333ef1e265f2a838c8b834a9ceb55f52a6aa0985a9bd9c6",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"151e4969-2f6e-4189-bc84-0d04784ca414",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"boatnewyork.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T08:47:50.4656718Z",
            //                 "FileTime":"2024-04-18T08:47:50.4657028Z",
            //                 "DeleteTime":null,
            //                 "FileSize":83445,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_5d6a5aae6ff51e29b24017ba1c3f37dae02e3f7a61586350f8cdcc71c58f1eef",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"2d18fab7-f685-4716-8020-439edd0f1570",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"buses.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T08:52:04.9466549Z",
            //                 "FileTime":"2024-04-18T08:52:04.9467025Z",
            //                 "DeleteTime":null,
            //                 "FileSize":18890,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_dceb4edcd50c964c7843bda50c9193f45c54fd2b3caab036abd16a4e42205987",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"67162096-2b2b-454d-8f4d-18c19089196c",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"cruise.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T08:54:57.7979117Z",
            //                 "FileTime":"2024-04-18T08:54:57.7979133Z",
            //                 "DeleteTime":null,
            //                 "FileSize":11467,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //    {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_fdb384f56e660d941c204bf8edb1faa566818b5e657d90a13417ff37dae69c27",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"a3ef1d7d-dcb8-4f70-b15d-ad2c95030ddd",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"fastbus.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T08:57:59.2577946Z",
            //                 "FileTime":"2024-04-18T08:57:59.2578605Z",
            //                 "DeleteTime":null,
            //                 "FileSize":16898,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //                 }
            //     } 
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_b475243b698438105669e07ab16dd555acfdff43f486115e7751731b3347e091",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"17187abb-7beb-4bab-b284-cdac60e61dae",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"helicopter.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:00:28.0976686Z",
            //                 "FileTime":"2024-04-18T09:00:28.0976945Z",
            //                 "DeleteTime":null,
            //                 "FileSize":10398,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_646d13747c3323d80ba4b357de59d483c93dfb899dc7670a8e94dfc76e8b284c",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"b56374f5-79a7-4882-8448-32a0d714ea88",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"kitelaunch.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:03:57.5800404Z",
            //                 "FileTime":"2024-04-18T09:03:57.5800747Z",
            //                 "DeleteTime":null,
            //                 "FileSize":8390,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_cdda1f3fec4ce3531cd5a7b7919001261482ba609535aa85f88d4f11d688ec93",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"b5db3422-77aa-4033-84a6-58256ce10a22",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"offroadcar.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:06:13.6784926Z",
            //                 "FileTime":"2024-04-18T09:06:13.678518Z",
            //                 "DeleteTime":null,
            //                 "FileSize":7470,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_1e9305ba2880d31682ffa0e5170cca0f531300e036bc9f310636806f67e7264d",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"81c88e1d-f23c-413b-abf7-172bdf02e62c",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"picnic.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:08:16.0080146Z",
            //                 "FileTime":"2024-04-18T09:08:16.0080447Z",
            //                 "DeleteTime":null,
            //                 "FileSize":6368,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_c4d8b0d3b3e1007e9e5c44e22a990912c52f0bb67ad32a77e298c5b1b0b15d8b",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"3ca02b3e-f707-4995-ad0c-8b5bbbafbb84",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"planelanding.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:10:19.4431933Z",
            //                 "FileTime":"2024-04-18T09:10:19.4432419Z",
            //                 "DeleteTime":null,
            //                 "FileSize":25263,"
            //                 PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_1273cdc39db86ab1534c86728ce76685fad3ab9003ace68e138fd74191f37394",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"139635d9-5e1f-4e1e-a924-5b8cf9c780ce",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"redtram.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:12:30.1075999Z",
            //                 "FileTime":"2024-04-18T09:12:30.1076015Z",
            //                 "DeleteTime":null,
            //                 "FileSize":7646,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_c54bb8f34238215f8255c91dfc7ba81850c044b891e34a87778230a252cdabe7",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"f871a4ac-b8aa-4df4-b9a4-35bed03523e9",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"redvantrip.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:14:55.4035704Z",
            //                 "FileTime":"2024-04-18T09:14:55.40361Z",
            //                 "DeleteTime":null,
            //                 "FileSize":50126,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_b1ac62b5d21e54c8d079ded91321a67bea03527c3004214fd0b8dab07031d1b3",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"41c37209-0c56-4c5b-a34d-2e5d241c5734",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"skiing.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:16:57.9061636Z",
            //                 "FileTime":"2024-04-18T09:16:57.906165Z",
            //                 "DeleteTime":null,
            //                 "FileSize":32227,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_34dc4b67e1ffc4a2ce5331c9c1b87db68d935fc6291bb3afc2b6366f03c79796",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"709e0b7f-684f-4b61-8307-10aa2aae4ae8",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"snownight.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:19:21.5664044Z",
            //                 "FileTime":"2024-04-18T09:19:21.5664354Z",
            //                 "DeleteTime":null,
            //                 "FileSize":58688,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":false,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_021787d8d3a2dfef4fef59054c9b0bb16a9279f016917a7efd7cb0c0c5037260",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"0389c6fa-4f7a-437e-8563-9b181ab65c79",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"swisstrain.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:22:05.1991431Z",
            //                 "FileTime":"2024-04-18T09:22:05.1991833Z",
            //                 "DeleteTime":null,
            //                 "FileSize":117203,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_43c31303d75b93b6e8e314f701521e1baf9e5fd8303839a1735b643debf5edf8",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"e6fd5770-1d7d-414a-a17c-0efd5413de15",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"train.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:24:19.5156074Z",
            //                 "FileTime":"2024-04-18T09:24:19.5156091Z",
            //                 "DeleteTime":null,
            //                 "FileSize":6537,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_20fd8addd4069824d44377a2c9ea4aa44e43a66e3e578b89787b039e0f6c88f7",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"b00f596f-bd9a-47a2-ba76-a3bc2e1f8aba",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"walk.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:26:59.1134454Z",
            //                 "FileTime":"2024-04-18T09:26:59.1134842Z",
            //                 "DeleteTime":null,
            //                 "FileSize":34783,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_3be1dd951c3bed8327b54697a42515e5d13698c8c57efc7dd26dbc08727a83c7",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"9987fe67-d7d8-426d-922a-57b0d3cbaca5",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"waterstream.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:29:32.7328898Z",
            //                 "FileTime":"2024-04-18T09:29:32.7329193Z",
            //                 "DeleteTime":null,
            //                 "FileSize":61457,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":true,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
            // ,
            // """
            //     {
            //         "asset":
            //             {
            //                 "PublicId":"53b4aac5ad6e-6f650de4-3db5-4de7-ab0c-300ff4f51e7e-c89c6826a6f069cda89340456ff35b47d57a666a5a4396eaf5a05e4b80a4988a_7bc3ef80f810b959cfa9ff855ccc2bd0262e617022ee486054f6a59331182012",
            //                 "Archive":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "FolderPath":"53b4aac5ad6e_6f650de4-3db5-4de7-ab0c-300ff4f51e7e",
            //                 "InternalId":"0b8d0135-dac6-4446-8f02-557e75e230d5",
            //                 "TenantId":null,
            //                 "FolderId":null,
            //                 "Renditions":null,
            //                 "DisplayFilename":"jump.mp4",
            //                 "FileExtension":"mp4",
            //                 "FileType":"MPEG",
            //                 "AssetType":0,
            //                 "CreatedTime":"2024-04-18T09:32:02.3543671Z",
            //                 "FileTime":"2024-04-18T09:32:02.3544113Z",
            //                 "DeleteTime":null,
            //                 "FileSize":157847,
            //                 "PageCount":0,
            //                 "AdditionalData":{},
            //                 "uploadedWithSidecar":false,
            //                 "indexTimeUtc":null,
            //                 "isLargeFile":false
            //             }
            //     }
            // """
        };        
    }

    private static List<string> GetMessages()
    {
        return new List<string>()
        {
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "bb5ab2dd-f89c-4689-976b-0de2fce614ec",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "walk",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "walk_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "walk_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "227ac968-7f98-41b5-806c-cd966f41128c",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "beach",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "beach_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "beach_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "a3a45e98-a2ce-4b37-84ab-3fbd1cae624f",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "friendseating",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "friendseating_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "friendseating_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "1e049c87-ce56-4c54-afc8-0c5a01a97bf3",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "boatnewyork",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "boatnewyork_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "boatnewyork_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "66403f95-2ded-496c-bb50-38713bbe2000",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "waterstream",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "waterstream_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "waterstream_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "40f88882-73fd-4785-b662-9c87c2876814",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "strangerthings",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/420",
                        "outFileName": "strangerthings_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "strangerthings_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "693cd80f-e1f0-4d42-b632-88a6829b196d",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "snownight",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "snownight_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "snownight_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "e9e398c9-222e-454d-8e88-c532be6d5842",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "basketball",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "basketball_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "basketball_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "e34a31a8-a169-47cb-b2ac-cf5edcdf444e",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "jump",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "jump_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "jump_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "8299d8a0-0bd2-4024-be05-121edd2bba29",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "kitelaunch",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "kitelaunch_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "kitelaunch_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "048e8847-4eca-490d-a854-e11b14d97483",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "babyandmother",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "babyandmother_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "babyandmother_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "28e35a07-9290-42aa-ab2f-70cfb0598b4c",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "picnic",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "picnic_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "picnic_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "e41e09f4-c912-414b-8f57-ba0fc3de9b27",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "helicopter",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "helicopter_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "helicopter_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "443a0934-b7a8-46ae-b9ac-c9f09943fc93",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "bicycle",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "bicycle_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "bicycle_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "99c8359e-e08c-4222-bac9-8fa79a67e969",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "offroadcar",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "offroadcar_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "offroadcar_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "67eb0e56-2f3e-493d-bb5a-fcc86bf99c63",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "redvantrip",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "redvantrip_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "redvantrip_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "5fbdad39-feb5-4e23-b217-5afa22e47092",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "fastbus",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "fastbus_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "fastbus_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "aa247810-426d-4df1-9718-477153bcd97f",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "buses",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/4",
                        "outFileName": "buses_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "buses_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "f7e333b0-756b-4fe4-9845-cd56bd6f6a00",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "train",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "train_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "train_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "8ff1040f-5d54-4586-8a9d-e8a904697d77",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "redtram",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "redtram_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "redtram_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "c693d32f-8fa3-46d7-a856-a4e15ed68bb2",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "swisstrain",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/3",
                        "outFileName": "swisstrain_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "swisstrain_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "1433e364-a19a-404d-8212-7c2ac151d8f8",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "planelanding",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/3",
                        "outFileName": "planelanding_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "planelanding_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "53b8d27a-d837-4a33-872f-2e5c9c7c6844",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "cruise",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/3",
                        "outFileName": "cruise_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 1280x720",
                        "outFileName": "cruise_720p.mp4"
                        }
                    ]
                }
            """,
            """
                {
                    "assetContainerName": "originals-de1885b94150-d6f6b9f9-f2eb-42cf-96c5-fe0be098fef3",
                    "assetId": "404179ec-f3ad-48f6-9de0-33b6b176358e",
                    "originalAssetBlobName": "original",
                    "sourceStorageAccount": "cheuw001assetsstcool",
                    "destinationStorageAccount": "cheuw001assetssthot",
                    "outFilePrefix": "skiing",
                    "commandArgs": [
                        {
                        "outFileOptions": "-vf fps=1/2",
                        "outFileName": "skiing_%04d.png"
                        },
                        {
                        "outFileOptions": "-vcodec libx264 -crf 28 -preset ultrafast -c:a copy -s 720x1280",
                        "outFileName": "skiing_720p.mp4"
                        }
                    ]
                }
            """
        };
    }
}
