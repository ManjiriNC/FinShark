using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockrepo;

        private readonly IPortfoliorepository _portrepo;
        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockrepo,IPortfoliorepository portrepo)
        {
            _userManager = userManager;
            _stockrepo =stockrepo;
            _portrepo = portrepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.Getusername();
            var appUser =  await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portrepo.GetUserPortfolio(appUser);

            return Ok(userPortfolio);
        }

        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.Getusername();
            var appUser = await _userManager.FindByNameAsync(username);
            var stock = await _stockrepo.GetBySymbolAsync(symbol);

            if(stock == null) return BadRequest("Stock not Found");

            var userPortfolio = await _portrepo.GetUserPortfolio(appUser);

            if(userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower())) 
               return BadRequest("Cannot add same stock to portfolio");
            
            var portfolio = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = appUser.Id
            };

            await _portrepo.CreateAsync(portfolio);

            if(portfolio == null)
            {
                return StatusCode(500, "Could not Create");
            }
            else{
                return Created();
            } 
        }


        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.Getusername();
            var appUser = await _userManager.FindByEmailAsync(username);

            var userPortfolio = await _portrepo.GetUserPortfolio(appUser);

            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();

            if(filteredStock.Count()== 1)
            {
                await _portrepo.DeletePortfolio(appUser,symbol);
            }
            else{
                return BadRequest("Stock is not in your portfolio");
            }

            return Ok();
        }
    }
}