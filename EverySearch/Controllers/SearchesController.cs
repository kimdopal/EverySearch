﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EverySearch.Lib;
using EverySearch.Models;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;

namespace EverySearch.Controllers
{
    public class SearchesController : Controller
    {
        private readonly EverySearchContext context;
        private readonly SearchManager searchManager;

        public SearchesController(EverySearchContext context, SearchManager searchManager)
        {
            this.context = context;
            this.searchManager = searchManager;
        }

        public IActionResult New(string query = "")
        {
            return View("New", query);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(string query)
        {
            Search search = new Search();
            search.Query = query;
            search.Timestamp = DateTime.Now;
            context.Searches.Add(search);
            var result = await searchManager.ExecuteQueryAsync(query, 10);
            foreach (var item in result)
            {
                item.Search = search;
                context.SearchResults.Add(item);
            }
            context.SaveChanges();
            return RedirectToAction("Show", new { search.Id });
        }

        public async Task<IActionResult> ShowAsync(int Id)
        {
            Search search = await context.Searches.Include(s => s.SearchResults).SingleAsync(s => s.Id == Id);
            if (search.SearchResults.Count == 0)
                return View("Error", Url.Action("New", new { query = search.Query}));
            return View(search);
        }
    }
}