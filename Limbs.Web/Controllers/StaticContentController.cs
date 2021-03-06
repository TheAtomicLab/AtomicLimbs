﻿using System;
using System.Web.Mvc;

namespace Limbs.Web.Controllers
{
    public class StaticContentController : Controller
    {
        // GET: StaticContent/faq
        public ActionResult Faq()
        {
            return View();
        }

        // GET: StaticContent/ManualEmbajador
        public ActionResult ManualEmbajador()
        {
            return View();
        }

        // GET: StaticContent/QueEsLimbs
        public ActionResult QueEsLimbs()
        {
            if (Request.IsAuthenticated)
            {
                return View("QueEsLimbs");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: StaticContent/PageNotFound
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            if (Request.IsAjaxRequest())
                return new HttpStatusCodeResult(404);
            return View();
        }

        // GET: StaticContent/Error
        public ActionResult Error()
        {
            Response.StatusCode = 500;
            if(Request.IsAjaxRequest())
                return new HttpStatusCodeResult(500);
            return View("Error");
        }

        // GET: StaticContent/TestError
        public ActionResult TestError()
        {
            throw new EntryPointNotFoundException("TestException", new Exception("Inner TestException"));
        }

        // GET: StaticContent/Redirect
        public ActionResult Redirect()
        {
            Response.StatusCode = 200;
            return new HttpStatusCodeResult(200);
        }

        // GET: StaticContent/Donar
        public ActionResult Donar()
        {
            return RedirectPermanent("http://AtomicLab.org/donar");
        }

        //// GET: StaticContent/Faq
        //public ActionResult Faq()
        //{
        //    return RedirectPermanent("http://AtomicLab.org/faq");
        //}

        // GET: StaticContent/Libre
        public ActionResult Libre()
        {
            return RedirectPermanent("http://AtomicLab.org/libre");
        }

        // GET: StaticContent/Manoton
        public ActionResult Manoton()
        {
            return RedirectPermanent("http://AtomicLab.org/manoton");
        }

        // GET: StaticContent/Embajadores
        public ActionResult Embajadores()
        {
            return RedirectPermanent("http://AtomicLab.org/Embajadores");
        }

        // GET: StaticContent/Animales
        public ActionResult Animales()
        {
            return RedirectPermanent("http://AtomicLab.org/animales");
        }
    }
}