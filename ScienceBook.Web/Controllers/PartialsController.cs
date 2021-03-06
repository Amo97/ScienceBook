﻿using ScienceBook.Web.DAL;
using ScienceBook.Web.Models.DbModels;
using ScienceBook.Web.Models.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScienceBook.Web.Controllers
{
    [Authorize]
    public class PartialsController : Controller
    {
        ScienceBookContext db = new ScienceBookContext();
        // GET: Partials
        public ActionResult GetUserInfo()
        {
            var member = db.Members.Where(m => m.Email.Equals(User.Identity.Name)).FirstOrDefault();
            ViewBag.Username = member.FirstName + member.LastName;
            ViewBag.Member = member;
            return PartialView("_UserInfo");
        }

        public ActionResult GetUserScienceClubs()
        {
            var scienceClubs = db.Members.Where(m => m.Email.Equals(User.Identity.Name)).FirstOrDefault().ScienceClubs;
            var images = new List<string>();
            foreach (var item in scienceClubs)
            {
                if (item.Logo != null)
                    images.Add(Imager.ByteArrayToStringImage(item.Logo));
                else if (!item.LogoS.Equals(""))
                    images.Add(item.LogoS);
                else
                    images.Add($"https://avatars.dicebear.com/api/jdenticon/{item.Name}.svg");
            }
            ViewBag.ScienceClubs = scienceClubs;
            ViewBag.Images = images;
            return PartialView("_UserScienceClubs");
        }

        public ActionResult GetScienceClubMembers(int id)
        {
            var scienceClub = db.ScienceClubs.Find(id);
            var members = scienceClub.Members;
            var logos = new List<string>();
            var roles = new List<string>();

            foreach (var item in members)
            {
                logos.Add(Imager.ByteArrayToStringImage(item.Logo));
                roles.Add(db.ScienceClubs_Members_Roles
                            .Where(smr => smr.MemberID == item.ID && smr.Role.ScienceClubID == id)
                            .FirstOrDefault()
                            .Role.Name);
            }
            ViewBag.Roles = roles;
            ViewBag.Logos = logos;
            ViewBag.Members = members;

            return PartialView("_ScienceClubMembers");
        }

        public ActionResult UniversityAcademies(string name)
        {
            ViewBag.Name = name;
            if(name != null)
            {
                var acaList = db.Departments.Where(a => a.University.Name.Equals(name)).ToList();
                ViewBag.Academies = acaList;
            }
            return PartialView();
        }

        public ActionResult ScienceClubElection(int scienceClubID, int election)
        {
            var sc = db.ScienceClubs.Find(scienceClubID);
            var ele = db.Elections.Where(e => e.ScienceClubID == sc.ID && e.DayOfEnd >= DateTime.Now).ToList();
            if (ele.Count != 0)
            {
                ViewBag.Election = ele[election - 1];

                var votedList = db.Votes.ToList();
                votedList = votedList.Where(v => v.OptionsInElection.ElectionID == ele[election - 1].ID).ToList();
                votedList = votedList.Where(v => v.Member.Email.Equals(User.Identity.Name)).ToList();
                ViewBag.VotedList = votedList;

                var votes = db.Votes.ToList();
                votes = votedList.Where(v => v.OptionsInElection.ElectionID == ele[election - 1].ID).ToList();
                double[] percents = new double[ele[election - 1].OptionsInElection.Count];
                int interator = 0;
                foreach (var item in ele[election - 1].OptionsInElection)
                {
                    double temp = votes.Where(v => v.OptionsInElectionID == item.ID).ToList().Count;
                    percents[interator++] = Math.Round((temp / votes.Count) * 100, 2);
                }
                for (int i = 0; i < percents.Length; i++)
                {
                    if (double.IsNaN(percents[i]))
                    {
                        percents[i] = 0;
                    }
                }
                ViewBag.Percents = percents;
            }

            return PartialView("_ScienceClubElection");
        }

        public ActionResult ScienceClubPosts (int scienceClubID)
        {
            var sc = db.ScienceClubs.Find(scienceClubID);
            var posts = sc.Posts.OrderBy(p => p.PublicationDay).ToList();

            if (sc.Logo != null)
                ViewBag.Logo = Imager.ByteArrayToStringImage(sc.Logo);
            else if (!sc.LogoS.Equals(""))
                ViewBag.Logo = sc.LogoS;
            else
                ViewBag.Logo = $"https://avatars.dicebear.com/api/jdenticon/{sc.Name}.svg";

            if (posts.Count <= 5)
            {
                ViewBag.Posts = posts;
            }
            else
            {
                ViewBag.Posts = new List<Post>();
                for (int i = 0; i < 5; i++)
                {
                    ViewBag.Posts.Add(posts[i]);
                }
            }
            return PartialView("_ScienceClubPosts");
        }

        public ActionResult ScienceClubEvent(int? id)
        {
            var ev = db.Events.Find(id);
            var ml = db.MailingList.Where(m => m.ScienceClubID == ev.ScienceClubID).ToList();

            ViewBag.Event = ev;
            ViewBag.MailingList = ml;

            return PartialView("_ScienceClubEvent", ev);
        }
    }
}