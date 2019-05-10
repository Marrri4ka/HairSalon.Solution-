using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using HairSalon.Models;

namespace HairSalon.Controllers
{
public class CategoriesController : Controller
{
[HttpGet("/stylists")]
public ActionResult Index()
{
	List<Stylist> allStylists = Stylist.GetAll();
	return View(allStylists);
}

[HttpPost("/stylists")]
public ActionResult Create(string stylistName)
{
	Stylist newStylist = new Stylist(newStylist);
	newStylist.Save();
	List<Stylist> allStylists = Stylist.GetAll();
	return View("Index", allStylists);
}

[HttpGet("/stylists/new")]
public ActionResult New()
{
	return View();
}

[HttpGet("/stylists/{id}")]
public ActionResult Show(int id)
{
	Dictionary<string, object> model = new Dictionary<string, object>();
	Stylist selectedStylist = Stylist.Find(id);
	List<Client> stylistClient = selectedClient.GetClients();
	model.Add("category", selectedClient);
	model.Add("items", stylistClient);
	return View(model);
}

}
}