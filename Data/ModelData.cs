using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultipleUserLoginForm.Data
{
    public static class ModelData
    {

        public static List<Subject> GetSubjects()
        {
            using (var db = new ModelContext())
            {
                return db.Subjects.ToList();
            }
        }
        public static List<Model.Object> GetObjects()
        {
            using (var db = new ModelContext())
            {
                return db.Objects.ToList();
            }
        }

        public static void AddSubjectToDb(SubjectViewModel subjectViewModel)
        {
            if(subjectViewModel==null) { return; }
            Subject subject=subjectViewModel.GetSubject();
            using (var db= new ModelContext())
            {
                db.Add(subject);
                db.SaveChanges();
            }
        }

        public static void AddObjectToDb(ObjectViewModel objectViewModel)
        {
            if(objectViewModel==null) { return; }
            Model.Object obj = objectViewModel.GetObject();
            using (var db = new ModelContext())
            {
                db.Add(obj);
                db.SaveChanges();
            }
        }
        public static void ClearSubjectsDb()
        { 
            using (var db= new ModelContext())
            {
                db.Subjects.RemoveRange(db.Subjects.ToArray());
                db.SaveChanges();
            }
        }

        public static void ClearObjectsDb()
        {
            using (var db = new ModelContext())
            {
                db.Objects.RemoveRange(db.Objects.ToArray());
                db.SaveChanges();
            }
        }
        public static void ChangeSubjectToDb(SubjectViewModel subj, SubjectViewModel old)
        {
            if(subj==null || old==null) { return; }
            Subject subject=subj.GetSubject();
            Subject oldSubject=old.GetSubject();
            
            using (var db= new ModelContext())
            {
                Subject s = db.Subjects.Find(oldSubject.Id);
                if (s == null) { return; }

                s.Login = subject.Login ;
                s.Password = subject.Password ;
                s.SecurityMark=subject.SecurityMark;
                s.Name = subject.Name;
                s.SecondName=subject.SecondName;
                db.SaveChanges();
            }
        }

        public static void ChangeObjectToDb(ObjectViewModel obj, ObjectViewModel old)
        {
            Model.Object object_ = obj.GetObject();
            Model.Object oldObject = old.GetObject();
            using (var db = new ModelContext())
            {
                Model.Object o=db.Objects.Find(oldObject.Id);
                if (o == null) { return; }
                o.Name=object_.Name;
                o.Path= object_.Path;
                o.SecurityMark= object_.SecurityMark;
                db.SaveChanges();
            }
        }
        public static void RemoveSubjectToDb(SubjectViewModel subjectViewModel)
        {
            if(subjectViewModel == null) { return; }
            Subject subject=subjectViewModel.GetSubject();
            if (subject.Id > 0)
            {
                using (var db= new ModelContext())
                {
                    try
                    {   
                        db.Remove(subject);
                        db.SaveChanges();

                    }catch (Exception ex)
                    {
                       MessageBox.Show(ex.Message);
                    }
              
                }

            }
           
        }

        public static void RemoveObjectToDb(ObjectViewModel objectViewModel)
        {
            if(objectViewModel == null) { return; }
            Model.Object obj = objectViewModel.GetObject();
            if (obj.Id > 0)
            {


                using (var db = new ModelContext())
                {
                    try {  
                        db.Remove(obj);
                        db.SaveChanges();
                    } catch(Exception ex) {
                        MessageBox.Show(ex.Message);
                    }
                   
                }
            }
        }
    }
}
