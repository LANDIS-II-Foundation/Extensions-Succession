//  Copyright 2009 Conservation Biology Institute
//  Authors:  Robert M. Scheller
//  License:  Available at  
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

using Landis.Ecoregions;
using Landis.Landscape;
using Landis.Species;
using Landis.Biomass;  
using Landis.PlugIns;

using System;
using System.Collections.Generic;


namespace Landis.Extension.Succession.Century
{
    /// <summary>
    /// A helper class.
    /// </summary>
    public class FireReductions
    //: IDisturbance
    {
        private double woodReduction;
        private double litterReduction;
        //private static ActiveSite currentSite;
        //private static FireReductions singleton;
        /*
        //---------------------------------------------------------------------

        ActiveSite IDisturbance.CurrentSite
        {
            get {
                return currentSite;
            }
        }
        //---------------------------------------------------------------------

        PlugInType IDisturbance.Type
        {
            get {
                return new PlugInType("disturbance:fire");
            }
        }
        //---------------------------------------------------------------------

        static FireReductions()
        {
            singleton = new FireReductions();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reduces the biomass of cohorts that have been marked for partial
        /// reduction.
        //  Original stand is the originating stand in cases where there is stand spreading.
        /// </summary>
        public static void ReduceCohortBiomass(ActiveSite site)
        {
            currentSite = site;            
            SiteVars.SiteCohorts[site].DamageBy(singleton);
            
        }
        //---------------------------------------------------------------------

        public float[] Damage(ICohort cohort)
        {
            float[] fireReductionLiveBiomass = new float[2]{5.0f,5.0f};
        
            return fireReductionLiveBiomass;  // percent reduction
        }*/

        
        public double WoodReduction
        {
            get {
                return woodReduction; 
            }
            set {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Wood reduction due to fire must be between 0 and 1.0");
                woodReduction = value;
            }
               
        }
        public double LitterReduction
        {
            get {
                return litterReduction; 
            }
            set {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Litter reduction due to fire must be between 0 and 1.0");
                litterReduction = value;
            }
               
        }
        //---------------------------------------------------------------------
        public FireReductions()
        {
            this.WoodReduction = 0.0; 
            this.LitterReduction = 0.0;
        }
    }
    
    public class FireEffects
    {
        public static FireReductions[] ReductionsTable; 
        
        public FireEffects(int numberOfSeverities)
        {
            ReductionsTable = new FireReductions[numberOfSeverities+1];  //will ignore zero
            
            for(int i=0; i <= numberOfSeverities; i++)
            {
                ReductionsTable[i] = new FireReductions();
            }
        }
       

        //---------------------------------------------------------------------

        public static void Initialize(IParameters parameters)
        {
            ReductionsTable = parameters.FireReductionsTable; 
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes fire effects on litter, coarse woody debris, mineral soil, and charcoal.
        ///   No effects on soil organic matter (negligible according to Johnson et al. 2001).
        /// </summary>
        public static void ReduceLayers(byte severity, ActiveSite site)
        {
            //UI.WriteLine("   Calculating fire induced layer reductions...");
        
            double litterLossMultiplier = ReductionsTable[severity].LitterReduction;
            
            // Structural litter first
            
            double carbonLoss = SiteVars.SurfaceStructural[site].Carbon * litterLossMultiplier;
            double nitrogenLoss = SiteVars.SurfaceStructural[site].Nitrogen * litterLossMultiplier;
            double summaryNLoss = nitrogenLoss;
            
            SiteVars.SurfaceStructural[site].Carbon -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireEfflux[site]               += carbonLoss;
            
            SiteVars.SurfaceStructural[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen        += nitrogenLoss;
            
            // Metabolic litter

            carbonLoss = SiteVars.SurfaceMetabolic[site].Carbon * litterLossMultiplier;
            nitrogenLoss = SiteVars.SurfaceMetabolic[site].Nitrogen * litterLossMultiplier;
            summaryNLoss += nitrogenLoss;
            
            SiteVars.SurfaceMetabolic[site].Carbon  -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireEfflux[site]               += carbonLoss;
            
            SiteVars.SurfaceMetabolic[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen        += nitrogenLoss;
            
            // Surface dead wood

            double woodLossMultiplier = ReductionsTable[severity].WoodReduction;
            
            carbonLoss   = SiteVars.SurfaceDeadWood[site].Carbon * woodLossMultiplier;
            nitrogenLoss = SiteVars.SurfaceDeadWood[site].Nitrogen * woodLossMultiplier;
            summaryNLoss += nitrogenLoss;
            
            SiteVars.SurfaceDeadWood[site].Carbon   -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireEfflux[site]               += carbonLoss;
            
            SiteVars.SurfaceDeadWood[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen        += nitrogenLoss;

            SiteVars.MineralN[site] += summaryNLoss * 0.01;
            
        }
        //---------------------------------------------------------------------
        
        // Crown scorching is when a cohort loses its foliage but is not killed.
        public static double CrownScorching(ICohort cohort, byte siteSeverity)
        {
        
            int difference = (int) siteSeverity - cohort.Species.FireTolerance;
            double ageFraction = 1.0 - ((double) cohort.Age / (double) cohort.Species.Longevity);
            
            if(SpeciesData.Epicormic[cohort.Species])
            {
                if(difference < 0)
                    return 0.5 * ageFraction;
                if(difference == 0)
                    return 0.75 * ageFraction;
                if(difference > 0)
                    return 1.0 * ageFraction;
            }
            
            return 0.0;
        }

    }
}
