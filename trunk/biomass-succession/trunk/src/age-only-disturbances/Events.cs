//  Copyright 2006 University of Wisconsin-Madison
//  Author: Jimm Domingo, FLEL

using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using Landis.PlugIns;

namespace Landis.Biomass.Succession.AgeOnlyDisturbances
{
    /// <summary>
    /// The handlers for various type of events related to age-only
    /// disturbances.
    /// </summary>
    public static class Events
    {
        public static void CohortDied(object         sender,
                                      DeathEventArgs eventArgs)
        {
            PlugInType disturbanceType = eventArgs.DisturbanceType;
            PoolPercentages cohortReductions = Module.Parameters.CohortReductions[disturbanceType];

            ICohort cohort = eventArgs.Cohort;
            ActiveSite site = eventArgs.Site;
            int nonWoody = cohort.ComputeNonWoodyBiomass(site);
            int woody = (cohort.Biomass - nonWoody);

            int nonWoodyInput = ReduceInput(nonWoody, cohortReductions.Foliar);
            int woodyInput = ReduceInput(woody, cohortReductions.Wood);

            //ForestFloor.AddBiomass(woodyInput, nonWoodyInput, cohort.Species, site);
            ForestFloor.AddWoody(woodyInput, cohort.Species, site);
            ForestFloor.AddLitter(nonWoodyInput, cohort.Species, site);
        }

        //---------------------------------------------------------------------

        private static int ReduceInput(int     poolInput,
                                          Percentage reductionPercentage)
        {
            int reduction = (int) (poolInput * reductionPercentage);
            return (int) (poolInput - reduction);
        }

        //---------------------------------------------------------------------

        public static void SiteDisturbed(object               sender,
                                         DisturbanceEventArgs eventArgs)
        {
            PlugInType disturbanceType = eventArgs.DisturbanceType;
            PoolPercentages poolReductions = Module.Parameters.PoolReductions[disturbanceType];

            ActiveSite site = eventArgs.Site;
            SiteVars.WoodyDebris[site].ReduceMass(poolReductions.Wood);
            SiteVars.Litter[site].ReduceMass(poolReductions.Foliar);
        }
    }
}
