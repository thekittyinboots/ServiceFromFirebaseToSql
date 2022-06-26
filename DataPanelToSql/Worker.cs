using DataPanelToSql.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;

namespace DataPanelToSql
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var userInfoFromFb= await GetDataFromFirebase();

                 var userPanelInfo = ConvertToUserPanelIngo(userInfoFromFb);

                await SaveDataToSql(userPanelInfo);

                await Task.Delay(86400000, stoppingToken);
            }
        }

        private async Task<List<UserInfoFB>> GetDataFromFirebase()
        {
            try
            {
            var auth = _configuration.GetValue<string>("FireBase:Secret");
            var urlBase = _configuration.GetValue<string>("FireBase:BaseUrl");
            var firebaseClient = new FirebaseClient(
            urlBase,
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(auth)
            });
                DateTime date=DateTime.Now.AddDays(-1);
                string file= date.ToString("yyyyMMdd");

                var results = await firebaseClient
                  .Child("Pos")
                  .Child(file)
                  .OrderByKey()
                  .OnceAsync<object>();

                List<UserInfoFB> listInfo = new List<UserInfoFB>();
                foreach (var r in results)
                {
                   var usuario = JsonConvert.DeserializeObject<UserInfoFB>(r.Object.ToString());
                    if (usuario.Log_Day!= null)
                    {
                        var logDay= JsonConvert.DeserializeObject<Dictionary<string,object>>(usuario.Log_Day.ToString());
                        foreach (var day in logDay)
                        {
                          var acion= JsonConvert.DeserializeObject<Log>(day.Value.ToString());
                          usuario.Log.Add(acion);
                        }
                     listInfo.Add(usuario);
                    }  
                }

                return listInfo;
            }
            catch (Exception e)
            {
                _logger.LogError("Occurio un error: {error}", e.Message);
                throw;
            }
            
        }

        private List<UserPanelInfo> ConvertToUserPanelIngo(List<UserInfoFB> list)
        {
            List<UserPanelInfo> info= new List<UserPanelInfo>();

            try
            {
                foreach (var user in list)
                {

                    foreach (var action in user.Log)
                    {
                        UserPanelInfo infoItem = new UserPanelInfo();
                        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        DateTime date = startTime.AddMilliseconds(action.Time).ToLocalTime();

                        infoItem.IdUsuario = user.Id;
                        infoItem.Usuario = user.Name;
                        infoItem.Fecha = date;
                        infoItem.Tipo = action.Type;
                        info.Add(infoItem);
                    }
                }
                return info;
            }
            catch (Exception e)
            {
                _logger.LogError("Occurio un error: {error}", e.Message);
                throw;
            }
            
        }

        private async Task SaveDataToSql(List<UserPanelInfo>  userPanelInfo)
        {
            try
            {
            using (var context = new ServiceContext(_configuration))
            {
                foreach (var info in userPanelInfo)
                {
                    if (info.IdUsuario >0)
                    {
                       var existingUser = context.UserPanelInfos.Where(x => x.IdUsuario == info.IdUsuario && x.Fecha.Year == info.Fecha.Year
                       && x.Fecha.Month == info.Fecha.Month && x.Fecha.Day == info.Fecha.Day && x.Fecha.Hour == info.Fecha.Hour && x.Fecha.Minute == info.Fecha.Minute && x.Tipo == info.Tipo).FirstOrDefault();

                            if (existingUser == null)
                            {
                                await context.UserPanelInfos.AddAsync(info);
                            }
                    }
                    
                }
               await  context.SaveChangesAsync();
            }
            }
            catch (Exception e)
            {
                _logger.LogError("Occurio un error: {error}", e.Message);
                throw;
            }
            

        }
    }
}