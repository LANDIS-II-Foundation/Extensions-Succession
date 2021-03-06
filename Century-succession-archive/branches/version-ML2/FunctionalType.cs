//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Ben Sulman

using Edu.Wisc.Forest.Flel.Util;
using Landis.SpatialModeling;
using Landis.Core;
using System.Collections.Generic;


namespace Landis.Extension.Succession.Century
{
    public interface IFunctionalType
    {
        double PPDF1{get;set;}
        double PPDF2{get;set;}
        double PPDF3{get;set;}
        double PPDF4{get;set;}
        double FCFRACleaf{get;set;}
        double BTOLAI{get;set;}
        double KLAI{get;set;}
        double MAXLAI{get;set;}
        double PPRPTS2 {get;set;}
        double PPRPTS3 {get;set;}
        double CoarseRootLignin {get;set;}
        double FineRootLignin {get;set;}
        double CoarseRootCN {get;set;}
        double MonthlyWoodMortality{get;set;}
        double WoodDecayRate{get;set;}
        double MortCurveShape{get;set;}
        int LeafNeedleDrop{get;set;}

    }
    
    public class FunctionalType
    : IFunctionalType
    {
        private double ppdf1;
        private double ppdf2;
        private double ppdf3;
        private double ppdf4;
        private double fcfracLeaf;
        private double btolai;
        private double klai;
        private double maxlai;
        private double pprpts2;
        private double pprpts3;
        private double coarseRootLignin;
        private double fineRootLignin;
        private double coarseRootCN;
        private double fineRootCN;
        private double monthlyWoodMortality;
        private double woodDecayRate;
        private double mortCurveShape;
        private int leafNeedleDrop;

        public static FunctionalTypeTable Table;
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Optimum temperature for production for parameterization of a Poisson Density Function 
        /// curve to simulate temperature effect on growth.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double PPDF1
        {
            get {
                return ppdf1;
            }
            set {
                    if (value  < 10.0 || value  > 40.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 10 and 40.0");
                ppdf1 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Maximum temperature for production for parameterization of a Poisson Density Function 
        /// curve to simulate temperature effect on growth.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double PPDF2
        {
            get {
                return ppdf2;
            }
            set {
                    if (value  < 20.0 || value  > 100.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 20 and 100.0");
                ppdf2 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Left curve shape for parameterization of a Poisson Density Function curve to 
        /// simulate temperature effect on growth.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double PPDF3
        {
            get {
                return ppdf3;
            }
            set {
                    if (value  < 0.0 || value  > 5.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 0 and 5.0");
                ppdf3 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Right curve shape for parameterization of a Poisson Density Function 
        /// curve to simulate temperature effect on growth.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double PPDF4
        {
            get {
                return ppdf4;
            }
            set {
                    if (value  < 0.0 || value  > 10.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 0 and 10.0");
                ppdf4 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// C allocation fraction of old leaves for mature forest.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double FCFRACleaf
        {
            get {
                return fcfracLeaf;
            }
            set {
                    if (value  < 0.0 || value  > 1.0)
                        throw new InputValueException(value.ToString(),
                            "The fraction of NPP allocated to leaves must be between 0 and 1.0");
                fcfracLeaf = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Biomass to leaf area index (LAI) conversion factor for trees.  This is a biome-specific parameters.  
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double BTOLAI
        {
            get {
                return btolai;
            }
            set {
                    if (value  < 0.00001 || value  > 2.5)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 0.00001 and 0.1");
                btolai = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Large wood mass in grams per square meter (g C /m2) at which half of the 
        /// theoretical maximum leaf area (MAXLAI) is achieved.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double KLAI
        {
            get {
                return klai;
            }
            set {
                    if (value  < 1.0 || value  > 10000.0)
                        throw new InputValueException(value.ToString(),
                            "K LAI must be between 1 and 10000");
                klai = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// The Century manual recommends a maximum of 20 (?)
        /// </summary>
        public double MAXLAI
        {
            get {
                return maxlai;
            }
            set {
                    if (value  < 0 || value  > 50.0)
                        throw new InputValueException(value.ToString(),
                            "Max LAI must be between 1 and 100");
                maxlai = value;
            }
        }
        //---------------------------------------------------------------------
        // 'PPRPTS(2)': The effect of water content on the intercept, allows the user to 
        //              increase the value of the intercept and thereby increase the slope of the line.
        public double PPRPTS2
        {
            get {
                return pprpts2;
            }
            set {
                pprpts2 = value;
            }
        }
        //---------------------------------------------------------------------
        // 'PPRPTS(3)': The lowest ratio of available water to PET at which there is no restriction on production.
        public double PPRPTS3
        {
            get {
                return pprpts3;
            }
            set {
                pprpts3 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Coarse Root Lignin Percent.
        /// </summary>
        public double CoarseRootLignin
        {
            get
            {
                return coarseRootLignin;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(),
                        "Coarse Root Lignin Percentage must be between 0 and 1.0");
                coarseRootLignin = value;
            }
        }
        
//---------------------------------------------------------------------
        /// <summary>
        /// Fine Root Lignin Percent.
        /// </summary>
        public double FineRootLignin
        {
            get
            {
                return fineRootLignin;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(),
                        "Fine Root lignin Percentage must be between 0 and 1.0");
                fineRootLignin = value;
            }
        }

//---------------------------------------------------------------------
        /// <summary>
        /// Coarse Root C:N ratio
        /// </summary>
        public double CoarseRootCN
        {
            get
            {
                return coarseRootCN;
            }
            set
            {
                if (value < 0.0 || value > 150.0)
                    throw new InputValueException(value.ToString(),
                        "Coarse Root CN ratio must be between 0 and 150.0");
                coarseRootCN = value;
            }
        }

//---------------------------------------------------------------------
        /// <summary>
        /// Coarse Root C:N ratio
        /// </summary>
        public double FineRootCN
        {
            get
            {
                return fineRootCN;
            }
            set
            {
                if (value < 0.0 || value > 150.0)
                    throw new InputValueException(value.ToString(),
                        "Fine Root CN ratio must be between 0 and 150.0");
                fineRootCN = value;
            }
        }
        //---------------------------------------------------------------------
        public double MonthlyWoodMortality
        {
            get {
                return monthlyWoodMortality;
            }
            set {
                    if (value  < 0.0 || value  > 1.0)
                        throw new InputValueException(value.ToString(),
                            "Monthly Wood Mortality is a fraction and must be between 0.0 and 1.0");
                monthlyWoodMortality = value;
            }
        }
        //---------------------------------------------------------------------
        public double WoodDecayRate 
        { 
            get { 
                return woodDecayRate;
            }
            set {
                    if (value  <= 0.0 || value  > 2.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 0.0 and 2.0");
                woodDecayRate = value;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines the shape of the age-related mortality curve.  Ranges from a gradual senescence (5)
        /// to a steep senescence (15).
        /// </summary>
        public double MortCurveShape 
        { 
            get { 
                return mortCurveShape;
            }
            set {
                    if (value  <= 2 || value  > 25)
                        throw new InputValueException(value.ToString(),
                            "Mortality shape curve parameters must be between 5 and 15");
                mortCurveShape = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines at what month of the year needles or leaves are dropped.
        /// </summary>
        public int LeafNeedleDrop
        {
            get { 
                return leafNeedleDrop;
            }
            set {
                    if (value  < 1 || value  > 12)
                        throw new InputValueException(value.ToString(),
                            "Leaf/Needle Drop must be a month of the year, 1-12");
                leafNeedleDrop = value;
            }
        }
        //---------------------------------------------------------------------

       public FunctionalType()
        {
        }
        
        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            Table = parameters.FunctionalTypes;
            //PlugIn.ModelCore.UI.WriteLine("  Functional Table [1].PPDF1={0}.", parameters.FunctionalTypeTable[1].PPDF1);
        }

        //---------------------------------------------------------------------
    }
}
