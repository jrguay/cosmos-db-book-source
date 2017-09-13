using CosmosUniversity.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CosmosUniversity.Web.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        [ActionName("Index")]
        public async Task<ActionResult> IndexAsync()
        {
            var students = await Repository<Student>.GetStudentsAsync(null);
            return View(students);
        }

        // GET: Student/Details/5
        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id, int pk)
        {
            var student = await Repository<Student>.GetStudentAsync(id, pk);
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(Student student)
        {
            if (!ModelState.IsValid)
                return View(student);

            try
            {
                await Repository<Student>.CreateStudentAsync(student);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(student);
            }
        }

        // GET: Student/Edit/5
        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id, int pk)
        {
            var student = await Repository<Student>.GetStudentAsync(id, pk);
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsnyc(string id, Student student)
        {
            if (!ModelState.IsValid)
                return View(student);

            try
            {
                await Repository<Student>.ReplaceStudentAsync(student, id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Student/Delete/5
        [ActionName("Delete")]
        public async Task<ActionResult> Delete(string id, int pk)
        {
            var student = await Repository<Student>.GetStudentAsync(id, pk);
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(string id, int pk, Student student)
        {
            try
            {
                await Repository<Student>.DeleteStudentAsync(id, pk);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(student);
            }
        }
    }
}
