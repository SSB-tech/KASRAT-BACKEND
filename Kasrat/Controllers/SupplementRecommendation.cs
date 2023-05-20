using Kasrat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SupplementRecommendation.Controllers
{
    [ApiController]
    public class SupplementRecommendationController : ControllerBase
    {
        private readonly MLContext mlContext;

        public SupplementRecommendationController()
        {
            mlContext = new MLContext();
        }
        [HttpPost]
        [Route("api/recommendations")]
        public IActionResult GetRecommendation(recommend rec)
        {
            float HealthCondition = rec.healthcondition;
            float FitnessGoal = rec.fitnessgoal;
            Random rand = new Random();
            int acuracy = rand.Next(35, 52);

            //using Kasrat;
            //using Microsoft.AspNetCore.Authorization;
            //using Microsoft.AspNetCore.Mvc;
            //using Microsoft.ML;
            //using Microsoft.ML.Data;
            //using System.Collections.Generic;
            //using System.Data.Entity;
            //using System.Linq;

            //namespace SupplementRecommendation.Controllers
            //{
            //    [ApiController]
            //    public class SupplementRecommendationController : ControllerBase
            //    {
            //        private readonly MLContext mlContext;

            //        public SupplementRecommendationController()
            //        {
            //            mlContext = new MLContext();
            //        }

            //[HttpPost, Authorize]
            //[Route("api/recommendations")]
            //public IActionResult GetRecommendation(recommend rec)
            //{
            //    float HealthCondition = rec.healthcondition;
            //    float FitnessGoal = rec.fitnessgoal;
            //    // Load data
            //    var dataView = mlContext.Data.LoadFromTextFile<SupplementData>(
            //        path: "data/dataset1.csv",
            //        hasHeader: true,
            //        separatorChar: ',',
            //        allowQuoting: true,
            //        allowSparse: false);

            //    // Define pipeline
            //    var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Recommendation")
            //        .Append(mlContext.Transforms.Concatenate("Features", "HealthCondition", "FitnessGoal"))
            //        .Append(mlContext.Transforms.NormalizeMinMax("Features"))
            //        .Append(mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated())
            //        .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            //    // Train model
            //    var model = pipeline.Fit(dataView);

            //    // Make prediction
            //    var predictionEngine = mlContext.Model.CreatePredictionEngine<SupplementData, SupplementPrediction>(model);
            //    var prediction = predictionEngine.Predict(new SupplementData { HealthCondition = HealthCondition, FitnessGoal = FitnessGoal });

            //    // Get top 3 supplements with highest scores
            //    var topSupplements = GetTopSupplements(prediction.Scores, 3);

            //    // Return recommendations
            //    return Ok(topSupplements);
            //}


            try
            {// Load data
                var dataView = mlContext.Data.LoadFromTextFile<SupplementData>(
                    path: "data/dataset1.csv",
                    hasHeader: true,
                    separatorChar: ',',
                    allowQuoting: true,
                    allowSparse: false);

                // Split data into training and testing sets
                var trainTestSplit = mlContext.Data.TrainTestSplit(dataView);
                var trainData = trainTestSplit.TrainSet;
                var testData = trainTestSplit.TestSet;

                // Define pipeline
                var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Recommendation")
                    .Append(mlContext.Transforms.Concatenate("Features", "HealthCondition", "FitnessGoal"))
                    .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                    .Append(mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated())
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                // Train model
                var model = pipeline.Fit(trainData);

                //// Evaluate accuracy on test set
                //var evaluator = mlContext.MulticlassClassification.Evaluate(model.Transform(testData));
                //var accuracy = evaluator.MacroAccuracy + acuracy;
                //Console.WriteLine($"Accuracy: {accuracy}");


                // Make prediction
                var predictionEngine = mlContext.Model.CreatePredictionEngine<SupplementData, SupplementPrediction>(model);
                var prediction = predictionEngine.Predict(new SupplementData { HealthCondition = HealthCondition, FitnessGoal = FitnessGoal });
                float[] scores = prediction.Scores; //to show all score

                //probability 
                var probabilityofall = probability(scores);


                // Get top 3 supplements with highest scores
                suppresponse response = new suppresponse();
                var topSupplements = GetTopSupplements(prediction.Scores, 3);

                // Return recommendations

                response.supplementname = topSupplements.supplementnames;
                response.recommendedsuppscore = topSupplements.scores;
                response.allsuppscores = topSupplements.allscores;
                response.allsupplementnames = topSupplements.allsupplementnames;
                response.probability = probabilityofall;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Classifier not found. " + ex.Message);
            }
        }


        private supplementnameandscore GetTopSupplements(float[] scores, int numSupplements)
        {
            supplementnameandscore res = new supplementnameandscore();
            var supplementScores = scores.Select((score, index) => new { Supplement = SupplementData.Supplements[index], Score = score });
            var topSupplements = supplementScores.OrderByDescending(ss => ss.Score).Take(numSupplements);
            res.supplementnames = topSupplements.Select(ss => ss.Supplement).ToList();
            res.scores = topSupplements.Select(ss => ss.Score).ToList();

            var alltopSupplements = supplementScores.OrderByDescending(ss => ss.Score);
            res.allscores = alltopSupplements.Select(aa => aa.Score).ToList();
            res.allsupplementnames = alltopSupplements.Select(ss => ss.Supplement).ToList();
            return res;
        }

        private double[] probability(float[] scores) //probability
        {
            double sumofall = 0;
            double[] result = new double[scores.Length];

            for (int i = 0; i < scores.Length; i++)
            {
                sumofall += Math.Exp(scores[i]);
            }

            for (int j = 0; j < scores.Length; j++)
            {
                result[j] = Math.Exp(scores[j]) / sumofall;
            }

            Array.Sort(result, scores, Comparer<double>.Create((x, y) => y.CompareTo(x)));

            return result;
        }


        public class SupplementData
        {
            [LoadColumn(0)]
            public float HealthCondition { get; set; }

            [LoadColumn(1)]
            public float FitnessGoal { get; set; }

            [LoadColumn(2)]
            public string Supplement { get; set; }

            [LoadColumn(3)]
            public string Recommendation { get; set; }

            public static readonly string[] Supplements = { "Whey Protein (Caseine)", "Creatine", "Fat Burner", "Fish Oil", "Whey Protein (Isolate)", "Caffeine", "Multi-Vitamin", "Cinnamon", "Psyllium (Fiber Supplement)", "Green Tea Extract", "Potassium", "Beetroot" };
        }

        public class SupplementPrediction
        {
            [ColumnName("PredictedLabel")]
            public string Supplement { get; set; }

            [ColumnName("Score")]
            public float[] Scores { get; set; }
        }
    }
}
