using UnityEngine;

namespace SpecialMaths {
    public partial class AngleSpace
    {
        private float theta;
        private float upB, lwB;
        private bool loop;

        //CONSTRUCTORS//
        public AngleSpace(float initial, float upperBound, float lowerBound, bool doesLoop)
        {
            
            upB = upperBound;
            lwB = lowerBound;
            loop = doesLoop;
            SetTheta(initial);
        }

        public AngleSpace(float initial, float upperBound, float lowerBound)
        {

            upB = upperBound;
            lwB = lowerBound;
            loop = true;
            SetTheta(initial);
        }
        //TOOOLS//
        public float getPropAngleSpace(float intialTheta, float targetTheta)
        {
            if (Mathf.Abs(targetTheta - intialTheta) > Mathf.Abs(targetTheta - (intialTheta + Mathf.PI * 2)))
            {
                targetTheta -= Mathf.PI * 2;
            }
            else if (Mathf.Abs(targetTheta - intialTheta) > Mathf.Abs(targetTheta - (intialTheta - Mathf.PI * 2)))
            {
                //Debug.Log("TEST2");
                targetTheta += Mathf.PI * 2;
            }
            return targetTheta;
        }
        public void fixTheta() {
            if (loop)
            {
                if (theta > upB) theta += lwB - upB;
                else if (theta < lwB) theta += -lwB + upB;
            }
            else {
                if (theta > upB) theta = upB;
                else if (theta < lwB) theta = lwB;
            }
        }
        public void StepLerp(float targetTheta, float smooth)
        {
            if (loop) targetTheta = getPropAngleSpace(theta, targetTheta);
            AddAngle((targetTheta - theta) / smooth);
        }
        public void LinearLerp(float intialTheta, float targetTheta, float time, float maxTime)
        {
            if (loop) targetTheta = getPropAngleSpace(intialTheta, targetTheta);
            SetTheta((targetTheta - intialTheta) * time / maxTime + intialTheta);
        }
        //SETTERS//
        public void SetTheta(float a1)
        {
            theta = a1;
            fixTheta();
        }
        public void AddAngle(float amount)
        {
            theta += amount;
            fixTheta();
        }
        //GETTERS//
        public float GetDeg()
        {
            return theta*Mathf.Rad2Deg;
        }
        public float GetFloat() {
            return theta;
        }
        public float GetLowerB(){
            return lwB;
        }
        public float GetUpperB() {
            return upB;
        }

        //overloads
        //ALSO FUTURE ME, Not sure if seting a1 to something is a big problem if it is, just make a a3 AngleSpace to placehold
        public static AngleSpace operator +(AngleSpace a1, float a2) {
            AngleSpace a3 = a1;
            a3.SetTheta(a1.GetFloat() + a2);
            return a3;
        }

        public static AngleSpace operator +(AngleSpace a1, AngleSpace a2) {
            AngleSpace a3 = new AngleSpace(0, a1.GetUpperB(), a1.GetLowerB());
            a3.SetTheta(a1.GetFloat() + a2.GetFloat());
            return a3;
        }

        public static AngleSpace operator *(AngleSpace a1, float a2) {
            AngleSpace a3 = new AngleSpace(0, a1.GetUpperB(), a1.GetLowerB());
            a3.SetTheta(a1.GetFloat() * a2);
            return a3;
        }


    }
}

