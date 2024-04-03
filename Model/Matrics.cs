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
            _objects = new List<Object>();
            _subjects = new List<Subject>();
        }
        private List<Subject> _subjects;
        public List<Subject> Subjects { get { return _subjects; } private set { _subjects = value;  } }

        private List<Object> _objects;
        public List<Object> Objects { get { return _objects; } private set { _objects = value; } }

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
       
        SecurityRight GetRight(Subject s,Object o)
        {
            SecurityRight right= null;
            switch (CurrentModelType)
            {
                case ModelType.BellaLapadula: right= GetBellaLapadulaModelRight(s,o); break;
                case ModelType.Biba: right = GetBibaModelRight(s,o); break;
                case ModelType.Combined: right = GetCombinedModelRight(s,o); break;

            }
            return right;
        }
        SecurityRight GetBellaLapadulaModelRight(Subject s, Object o)
        {
            Right right;
            if(s.SecurityMark > o.SecurityMark)
            {
                right = Right.Read;
            }
            else if(s.SecurityMark==o.SecurityMark){
                if (s.SecurityCategory > o.SecurityCategory)
                {
                    right = Right.Read;
                }
                else if (s.SecurityCategory == o.SecurityCategory)
                {
                    right = Right.All;
                }
                else { right = Right.Write; }
            }
            else
            {
                right = Right.Write;
            }
            return  new SecurityRight() { Object = o, Subject = s, Right = right };
            
        }
        SecurityRight GetBibaModelRight(Subject s, Object o)
        {
            Right right;
            if (s.SecurityMark > o.SecurityMark)
            {
                right = Right.Write;
            }
            else if (s.SecurityMark == o.SecurityMark)
            {
                if(s.SecurityCategory>o.SecurityCategory)
                {
                    right = Right.Write;
                }else if (s.SecurityCategory == o.SecurityCategory)
                {
                    right = Right.All;
                }
                else {
                    right = Right.Read;
                }
            }
            else
            {
                right = Right.Read;
            }
            return new SecurityRight() { Object = o, Subject = s, Right = right };

        }
        SecurityRight GetCombinedModelRight(Subject s, Object o)
        {
            Right right;
            if (s.SecurityMark == o.SecurityMark && s.SecurityCategory==o.SecurityCategory)
            {
                right = Right.All;
            }
            else
            {
                right = Right.None;
            }
            return new SecurityRight() { Object = o, Subject = s, Right = right };
        }
       
    }
}
