using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Model
{
    public class Matrics
    {
        public enum ModelType
        {
            BellaLapadula, Biba,Combined
        }
        private ModelType _modelType;

        public ModelType CurrentModelType
        {
            get { return _modelType; }
            set { _modelType = value; }
        }

        public Matrics() {
            objects = new List<Object>();
            subjects = new List<Subject>();
        }
        private List<Subject> subjects;
        private List<Object> objects;
        public List<Subject> Subjects { get { return subjects; } private set { subjects = value;  } }
        public List<Object> Objects { get { return objects; } private set { objects = value; } }

        public List<SecurityRight> Rights { get; private set; }
        public Subject GetSubject(string login)
        {
            return Subjects.Find(x => x.Login==(login));
        }
        public Object GetObject(string name)
        {
            return Objects.Find(x => x.Name.Equals(name));
        }
        public List<SecurityRight> GetObjectsForSubj(string login) { 


            Subject s = GetSubject(login);

            List<SecurityRight> l = new List<SecurityRight>();
            for (int j = 0; j < Objects.Count; ++j)
            {
                l.Add(GetRight(s, Objects[j]));
            }
            return l;
        }
       
        public void AddSubject(Subject subject)
        {
            Subjects.Add(subject);
           
        }
        public void RemoveSubject(Subject subject)
        {
            Subjects.Remove(subject);
           
        }
        public void AddObject(Object obj) {
            Objects.Add(obj); 
            
        }
        public void RemoveObject(Object obj)
        {
            Objects.Remove(obj);
           
        }
        SecurityRight GetRight(Subject s,Object o)
        {
            SecurityRight right= null;
            switch (CurrentModelType)
            {
                case ModelType.BellaLapadula: right= ImplyBellaLapaduleModel(s,o); break;
                case ModelType.Biba: right = ImplyBibaModel(s,o); break;
                case ModelType.Combined: right = ImplyCombinedModel(s,o); break;

            }
            return right;
        }
        SecurityRight ImplyBellaLapaduleModel(Subject s, Object o)
        {
            Right right;
            if(s.SecurityMark > o.SecurityMark)
            {
                right = Right.Read;
            }
            else if(s.SecurityMark==o.SecurityMark){
                right = Right.All;
            }
            else
            {
                right = Right.Write;
            }
            return  new SecurityRight() { Object = o, Subject = s, Right = right };
            
        }
        SecurityRight ImplyBibaModel(Subject s, Object o)
        {
            Right right;
            if (s.SecurityMark > o.SecurityMark)
            {
                right = Right.Write;
            }
            else if (s.SecurityMark == o.SecurityMark)
            {
                right = Right.All;
            }
            else
            {
                right = Right.Read;
            }
            return new SecurityRight() { Object = o, Subject = s, Right = right };

        }
        SecurityRight ImplyCombinedModel(Subject s, Object o)
        {
            Right right;
            if (s.SecurityMark > o.SecurityMark)
            {
                right = Right.Write;
            }
            else if (s.SecurityMark == o.SecurityMark)
            {
                right = Right.All;
            }
            else
            {
                right = Right.Read;
            }
            return new SecurityRight() { Object = o, Subject = s, Right = right };

        }
    }
}
