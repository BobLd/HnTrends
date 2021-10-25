﻿namespace HnTrends.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using System.Net;
    using ViewModels;

    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ITrendService trendService;

        public ApiController(ITrendService trendService)
        {
            this.trendService = trendService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok(trendService.GetTotalStoryCount());
        }

        [Route("results/{term}")]
        [HttpGet]
        public async Task<ActionResult> GetDataFull(string term, [FromQuery] bool allWords)
        {
            term = WebUtility.UrlDecode(term);

            term = SearchTermHelper.MakeSafeWordSearch(term, allWords);

            var full = await trendService.GetFullResultsForTermAsync(term)
                .ConfigureAwait(false);

            return Ok(full);
        }

        [Route("plot/{term}")]
        [HttpGet]
        public async Task<ActionResult> GetPlotAggregateData(string term, [FromQuery] bool allWords)
        {
            term = WebUtility.UrlDecode(term);

            var originalTerm = term;

            term = SearchTermHelper.MakeSafeWordSearch(term, allWords);

            var results = await trendService.GetTrendDataForTermAsync(term);

            return Ok(new PlotAggregateDataViewModel
            {
                Counts = results.Counts,
                Term = originalTerm,
                AllWords = allWords,
                Scores = results.Scores
            });
        }
    }
}
