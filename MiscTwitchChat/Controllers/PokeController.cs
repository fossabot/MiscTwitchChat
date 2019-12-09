﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiscTwitchChat.Helpers;
using MiscTwitchChat.Models;
using Newtonsoft.Json;

namespace MiscTwitchChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokeController : ControllerBase
    {
        private readonly MiscTwitchDbContext _db;
        private readonly ILogger _logger;

        public PokeController(ILogger<SpankController> logger, MiscTwitchDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("{channel}/{origUser}")]
        public async Task<string> HugAsync(string channel, string origUser)
        {
            _logger.LogInformation($"Starting poke from {origUser} in {channel}");
            string target = await TwitchApiClasslib.GetRandomConsentingChatter(_db, channel, origUser, "hug", false);

            return $"{target} was poke by {origUser}. POKE HARDER!";
        }

        [HttpGet("{channel}/{origUser}/consent")]
        public async Task<string> UpdateConsent(string channel, string origUser)
        {
            string rval = string.Empty;
            var disconsenter = _db.Disconsenters.FirstOrDefault(p => p.Name == origUser);
            if (disconsenter == null)
            {
                _db.Disconsenters.Add(new Disconsenter
                {
                    Name = origUser
                });
                rval = $"{origUser} has registered they do not consent.";
            }
            else
            {
                _db.Disconsenters.Remove(disconsenter);
                rval = $"{origUser} has registered that they DO consent.";
            }

            await _db.SaveChangesAsync();
            return rval;
        }
    }
}