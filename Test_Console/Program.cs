// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using RepositoryUOW.Data.Repositories;
using RepositoryUOW.Services.Http;
using RepositoryUOWDomain.Entities.System;
using RepositoryUOWDomain.Shared.Common;
using RepositoryUOWDomain.Shared.Enums;
using RepositoryUOWDomain.Shared.Extensions;
using RepositoryUOWDomain.ValueObject;

//User u = new()
//{
//    User_Security_ID = Guid.NewGuid(),
//    User_ID = "123NNN"
//};

//bool i = u.IsValid("User_ID");


//var n = new ApplicationDbContextFactory();
//var k = n.CreateDbContext(null);

//User u = new User()
//{
//    User_Security_ID = Guid.Parse("ba0b81b9-cb79-4011-88f4-5dfd89997e62"), // Guid.NewGuid(),
//    User_ID = "123NNN11111111",
//    Change_Password = true,
//    License_Type = UserLicenseTypeEnum.Full_User,
//    Expiry_Date = DateTime.Now,
//    State = UserStateEnum.Enabled,
//    User_FullName = "PPP",
//    Windows_Security_ID = "P23",
//    Contact_Email = "Nyyqase@ds.com"
//};

//BaseHttpClient vvvv = new();

//var ggg = await vvvv.Post<ResponseResult<bool>>("https://localhost:44355/WeatherForecast/SaveUser?isModify=1", u);


//RepositoryUow<User> uDate = new();




//List<User> uList = new();

//Enumerable.Range(1, 10000).ToList().ForEach(u =>
//{
//    uList.Add(new User()
//    {
//        User_Security_ID = Guid.NewGuid(),
//        User_ID = "123mmmmmmmm21212121",
//        Change_Password = true,
//        License_Type = UserLicenseTypeEnum.Full_User,
//        Expiry_Date = DateTime.Now,
//        State = UserStateEnum.Enabled,
//        User_FullName = "PPP",
//        Windows_Security_ID = "P34",
//        Contact_Email = "Nyyqase@ds.com"
//    });
//});


////int coreCount = 0;
////foreach (var item in new
////             System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get()){

////coreCount += int.Parse(item["NumberOfCores"].ToString());
////}
////Console.WriteLine("Number Of Cores: {0}", coreCount);
////Console.ReadLine();



////Console.WriteLine(u.IsValid());

//await uDate.SaveRange(uList.Where(w=>w != null).ToList(), InsertUpdateEnum.Insert);

//var gggg = (await uDate.AllItems()).ToList();

//bool l = await uDate.DeleteRange(uDate.DbEntity().Take(1000000));

//Console.WriteLine(uDate.ErrorMessage);

//await uDate.Save(new User()
//{
//    User_Security_ID = Guid.NewGuid(),
//    User_ID = "123NNN21212121.",
//    Change_Password = true,
//    License_Type = UserLicenseTypeEnum.Full_User,
//    Expiry_Date = DateTime.Now,
//    State = UserStateEnum.Enabled,
//    User_FullName = "12IIIII"

//}, InsertUpdateEnum.Insert);

//Console.WriteLine(uDate.ErrorMessage);



ConcurrentBagAsync<int> enumInt = new(Enumerable.Range(1, 100000).ToList());

ConcurrentBagAsync<int> enumIntNew = new();

//async Task Act(int i)
//{
//    await enumIntNew.Add(i).ConfigureAwait(false);
//}

//Func<int, Task> acto = Act;


Stopwatch sw = Stopwatch.StartNew();
await enumInt.RunExTask(async i => await enumIntNew.Add(i).ConfigureAwait(false));

//var block = new ActionBlock<int>(
//    async u => await enumIntNew.Add(u).ConfigureAwait(false), // What to do on each item
//    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 9 }); // How many items at the same time

//foreach (var item in enumInt)
//{
//    block.Post(item); // Post all items to the block
//}


//block.Complete(); // Signal completion
//await block.Completion; // Asynchronously wait for completion.

//await enumInt.RunExTaskSecond(async i => await enumIntNew.Add(i).ConfigureAwait(false));


//await enumInt.SelectAsync( async i => await enumIntNew.Add(i).ConfigureAwait(false) );

//enumInt.R
//await acto.RunExTask(enumInt.ToArray());
////Partitioner.Create(enumInt.ToList(), true).AsParallel().ForAll(async f => await Act(f));
////enumInt.ToList().AsParallel().ForAll(async f => await Act(f));
sw.Stop();
////Elapsed = {00:02:11.3973053} - enumInt.ToList().AsParallel().ForAll(async f => await Act(f));
////Elapsed = {00:02:00.8857906} - Partitioner.Create(enumInt.ToList(), true).AsParallel().ForAll(async f => await Act(f));
////Elapsed = {00:01:22.9876155} - await acto.RunExTask(enumInt.ToArray(), 12);
////Elapsed = {00:01:18.3353722} - await acto.RunExTask(enumInt.ToArray(),9);
///// Elapsed = {00:01:20.2841440} - enumInt.RunExTask(Act)
///// Elapsed = {00:13:16.6526408}
///// Elapsed = {00:18:16.7318596}
///// Target = Elapsed = {00:01:32.8605169}
///// Target = Elapsed = {00:01:23.9968563}
///// Elapsed = {00:16:35.4288629} - RunExTaskSecond
///// Elapsed = {00:17:42.6789767} - RunExTask

Console.WriteLine("Hello, World!");
 