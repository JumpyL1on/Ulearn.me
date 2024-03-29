﻿using System;

namespace MyPhotoshop
{
    public struct Pixel
    {
        private double r;
        public double R
        {
            get { return r; }
            set { r = CheckValue(value); }
        }

        private double g;
        public double G
        {
            get { return g; }
            set { g = CheckValue(value); }
        }

        private double b;
        public double B
        {
            get { return b; }
            set { b = CheckValue(value); }
        }

        public Pixel(double r, double g, double b) : this()
        {
            R = r;
            G = g;
            B = b;
        }

        public static Pixel operator *(Pixel p, double c)
        {
            return new Pixel(p.r * c, p.g * c, p.b * c);
        }

        public static Pixel operator *(double c, Pixel p)
        {
            return p * c;
        }

        private double CheckValue(double value)
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentException($"Wrong channel value {value} (the value must be between 0 and 1");
            }
            else
            {
                return value;
            }
        }
    }
}