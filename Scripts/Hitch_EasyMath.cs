using UnityEngine;

namespace Hitch_EasyMath
{
    public partial class AngleSpace {
        private float deg;
        private float upB, lwB;
        private bool loop;

        //construction
        public AngleSpace(float initial, float upperBound, float lowerBound, bool doesLoop) {
            upB = upperBound;
            lwB = lowerBound;
            loop = doesLoop;
            setDeg(initial);
        }

        public AngleSpace(float initial, float upperBound, float lowerBound) {
            upB = upperBound;
            lwB = lowerBound;
            loop = true;
            setDeg(initial);
        }

        public AngleSpace(float initial, bool doesLoop)
        {
            upB = 2 * Mathf.PI;
            lwB = 0;
            loop = doesLoop;
            setDeg(initial);
        }

        public AngleSpace(float initial) {
            upB = 2 * Mathf.PI;
            lwB = 0;
            loop = true;
            setDeg(initial);
        }



        //tools//
        public void lerp(float intialTheta, float targetTheta, float t)
        {
            if (loop) targetTheta = getPropAngleSpace(intialTheta, targetTheta);
            setDeg((targetTheta - intialTheta) * t + intialTheta);
        }
        public void lerp(float targetTheta, float t)
        {
            lerp(getDeg(), targetTheta, t);
        }
        public void fix() {
            if (loop)
            {
                if (deg > upB) deg += lwB - upB;
                else if (deg < lwB) deg += -lwB + upB;
            }
            else
            {
                if (deg > upB) deg = upB;
                else if (deg < lwB) deg = lwB;
            }
        }
        public void addAngle(float amount)
        {
            deg += amount;
            fix();
        }
        public void setDeg(float a1) {
            deg = a1;
            fix();
        }
        public void setTheta(float a1) {
            deg = a1 * Mathf.Rad2Deg;
            fix();
        }


        //getters//
        public float getPropAngleSpace(float intialTheta, float targetTheta)
        {
            if (Mathf.Abs(targetTheta - intialTheta) > Mathf.Abs(targetTheta - (intialTheta + (upB - lwB))))
            {
                targetTheta -= (upB - lwB);
            }
            else if (Mathf.Abs(targetTheta - intialTheta) > Mathf.Abs(targetTheta - (intialTheta - (upB - lwB))))
            {
                //Debug.Log("TEST2");
                targetTheta += (upB - lwB);
            }
            return targetTheta;
        }
        public float getDeg()
        {
            return deg;
        }
        public float getTheta()
        {
            return deg * Mathf.Deg2Rad;
        }
        public float getLowerB()
        {
            return lwB;
        }
        public float GetUpperB()
        {
            return upB;
        }

        //overloads//
        public static AngleSpace operator +(AngleSpace a1, float a2)
        {
            AngleSpace a3 = a1;
            a3.setTheta(a1.getTheta() + a2);
            return a3;
        }

        public static AngleSpace operator +(AngleSpace a1, AngleSpace a2)
        {
            AngleSpace a3 = new AngleSpace(0, a1.GetUpperB(), a1.getLowerB());
            a3.setTheta(a1.getTheta() + a2.getTheta());
            return a3;
        }

        public static AngleSpace operator *(AngleSpace a1, float a2)
        {
            AngleSpace a3 = new AngleSpace(0, a1.GetUpperB(), a1.getLowerB());
            a3.setTheta(a1.getTheta() * a2);
            return a3;
        }
    }
}

