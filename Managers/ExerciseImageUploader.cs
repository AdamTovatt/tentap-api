using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.IO;
using System.Threading.Tasks;
using TentaPApi.Helpers;
using TentaPApi.Models;

namespace TentaPApi.Managers
{
    public class ExerciseImageUploader
    {
        public string ProblemData { get; private set; }
        public string SolutionData { get; private set; }

        public string ProblemUuid { get; private set; }
        public string SolutionUuid { get; private set; }

        public CloudinaryImage ProblemImage { get; set; }
        public CloudinaryImage SolutionImage { get; set; }

        public ExerciseImageUploader(string problemData, string solutionData)
        {
            ProblemData = problemData;
            SolutionData = solutionData;
        }

        public async Task<bool> UploadImagesAsync()
        {
            Cloudinary cloudinary = CloudinaryHelper.GetCloudinary();

            ProblemUuid = Guid.NewGuid().ToString();
            SolutionUuid = Guid.NewGuid().ToString();

            ImageUploadParams problemImage = new ImageUploadParams();
            problemImage.File = new FileDescription(string.Format("q{0}", ProblemUuid), new MemoryStream(Convert.FromBase64String(ProblemData)));
            ImageUploadResult questionUploadResult = await cloudinary.UploadAsync(problemImage);

            ImageUploadParams solutionImage = new ImageUploadParams();
            solutionImage.File = new FileDescription(string.Format("s{0}", SolutionUuid), new MemoryStream(Convert.FromBase64String(SolutionData)));
            ImageUploadResult solutionUploadResult = await cloudinary.UploadAsync(solutionImage);

            ProblemImage = new CloudinaryImage() { Url = solutionUploadResult.Url.ToString() };
            SolutionImage = new CloudinaryImage() { Url = questionUploadResult.Url.ToString() };

            return true;
        }
    }
}
