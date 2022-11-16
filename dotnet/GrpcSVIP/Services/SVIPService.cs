using Grpc.Core;
using Newtonsoft.Json;
using OpenSvip.Model;

namespace GrpcSVIP.Services
{
    public class SVIPService : SVIP.SVIPBase
    {
        private readonly ILogger<SVIPService> _logger;
        public SVIPService(ILogger<SVIPService> logger)
        {
            _logger = logger;
        }

        public override Task<ModelReply> ProcessProject(SvipRequest request, ServerCallContext context)
        {
            Project? project = null;
            string message = "";
            string projectString = "";
            try
            {
                Dictionary<string, object> projectDict = new();
                projectDict["Version"] = request.Version;

                List<Dictionary<string, object>> songTempoList = new();
                foreach (var songTempo in request.SongTempoList)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(songTempo.ToString());
                    if (result != null)
                    {
                        songTempoList.Add(result);
                    }
                }
                projectDict["SongTempoList"] = songTempoList;

                List<Dictionary<string, object>> timeSignatureList = new();
                foreach (var timeSignature in request.TimeSignatureList)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(timeSignature.ToString());
                    if (result != null)
                    {
                        timeSignatureList.Add(result);
                    }
                }
                projectDict["TimeSignatureList"] = timeSignatureList;

                List<Dictionary<string, object>> trackList = new();
                string[] paramTypes = new string[] { "Pitch", "Volume", "Breath", "Gender", "Strength" };
                foreach (var track in request.TrackList)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(track.ToString());
                    if (result != null)
                    {
                        Dictionary<string, object> editedParams = new();
                        foreach (var paramType in paramTypes)
                        {
                            var param = new Dictionary<string, object>() { ["TotalPointsCount"] = 0, ["PointList"] = new List<long>() };
                            editedParams[paramType] = param;
                        }
                        result["EditedParams"] = editedParams;
                        trackList.Add(result);
                    }
                }
                projectDict["TrackList"] = trackList;

                projectString = JsonConvert.SerializeObject(projectDict);
                if (!String.IsNullOrWhiteSpace(projectString))
                {
                    project = JsonConvert.DeserializeObject<Project>(projectString);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Task.FromResult(new ModelReply
            {
                Message = project == null ? message : project.Version,
            });
        }
    }
}