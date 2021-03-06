//  Copyright 2007-2009 Conservation Biology Institute
//  Authors:  Robert M. Scheller
//  License:  Available at  
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using Landis.Succession;
using Landis.Species;
using Landis.Ecoregions;
using System.Collections.Generic;
using System.Diagnostics;

namespace Landis.Extension.Succession.Century
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public class Parameters
        : Dynamic.Parameters, IParameters
    {
        private int timestep;
        private SeedingAlgorithms seedAlg;
        private string climateFile;
        private bool calibrateMode;
        private double spinupMortalityFraction;
        public WaterType wtype;
        
        private FunctionalTypeTable functionalTypes;
        private FireReductions[] fireReductionsTable;
        
        private Species.AuxParm<int> sppFunctionalType;
        private Species.AuxParm<int> nTolerance;
        private Species.AuxParm<int> gddMin;
        private Species.AuxParm<int> gddMax;
        private Species.AuxParm<int> minJanTemp;
        private Species.AuxParm<double> maxDrought;
        private Species.AuxParm<double> leafLongevity;
        private Species.AuxParm<bool> epicormic;
        private Species.AuxParm<double> leafLignin;
        private Species.AuxParm<double> woodLignin;
        private Species.AuxParm<double> coarseRootLignin;
        private Species.AuxParm<double> fineRootLignin;
        private Species.AuxParm<double> leafCN;
        private Species.AuxParm<double> woodCN;
        private Species.AuxParm<double> coarseRootCN;
        private Species.AuxParm<double> foliageLitterCN;
        private Species.AuxParm<double> fineRootLitterCN;
        
        private Ecoregions.AuxParm<Percentage>[] minRelativeBiomass;
        private List<ISufficientLight> sufficientLight;
        
        private Ecoregions.AuxParm<double> percentClay;
        private Ecoregions.AuxParm<double> percentSand;
        private Ecoregions.AuxParm<int>    soilDepth;
        private Ecoregions.AuxParm<double> fieldCapacity;
        private Ecoregions.AuxParm<double> wiltingPoint;
        private Ecoregions.AuxParm<double> stormFlowFraction;
        private Ecoregions.AuxParm<double> baseFlowFraction;
        private Ecoregions.AuxParm<double> drain;
        //private Ecoregions.AuxParm<double> monthlyNDeposition;
        private Ecoregions.AuxParm<double> atmosNslope;
        private Ecoregions.AuxParm<double> atmosNintercept;
        private Ecoregions.AuxParm<double> latitude;

        private Ecoregions.AuxParm<double> initSOM1surfC;
        private Ecoregions.AuxParm<double> initSOM1surfN;
        private Ecoregions.AuxParm<double> initSOM1soilC;
        private Ecoregions.AuxParm<double> initSOM1soilN;
        private Ecoregions.AuxParm<double> initSOM2C;
        private Ecoregions.AuxParm<double> initSOM2N;
        private Ecoregions.AuxParm<double> initSOM3C;
        private Ecoregions.AuxParm<double> initSOM3N;
        private Ecoregions.AuxParm<double> initMineralN;

        
        private string ageOnlyDisturbanceParms;
        private List<Dynamic.ParametersUpdate> dynamicUpdates;

        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get {
                return timestep;
            }
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Timestep must be > or = 0");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Seeding algorithm
        /// </summary>
        public SeedingAlgorithms SeedAlgorithm
        {
            get {
                return seedAlg;
            }
            set {
                seedAlg = value;
            }
        }

        //---------------------------------------------------------------------

        public double SpinupMortalityFraction
        {
            get
            {
                return spinupMortalityFraction;
            }
            set
            {
                if (value < 0.0 || value > 0.5)
                    throw new InputValueException(value.ToString(), "SpinupMortalityFraction must be > 0.0 and < 0.5");
                spinupMortalityFraction = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Path to the required file with climatedata.
        /// </summary>
        public string ClimateFile
        {
            get {
                return climateFile;
            }
            set {
                //if (value != null) {
                    string path = value;
                    if (path.Trim(null).Length == 0)
                        throw new InputValueException(path,
                                                      "\"{0}\" is not a valid path.",
                                                      path);
                //}
                climateFile = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines whether months are simulated 0 - 12 (calibration mode) or
        /// 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 (normal mode with disturbance at June 30).
        /// </summary>
        public bool CalibrateMode
        {
            get {
                return calibrateMode;
            }
            set {
                calibrateMode = value;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines whether moisture effects on decomposition follow a linear or ratio calculation.
        /// </summary>
        public WaterType WType
        {
            get {
                return wtype;
            }
            set {
                wtype = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Functional type parameters.
        /// </summary>
        public FunctionalTypeTable FunctionalTypes
        {
            get {
                return functionalTypes;
            }
            set {
                functionalTypes = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fire reduction of leaf and wood litter parameters.
        /// </summary>
        public FireReductions[] FireReductionsTable
        {
            get {
                return fireReductionsTable;
            }
            set {
                fireReductionsTable = value;
            }
        }
        //---------------------------------------------------------------------

        public Ecoregions.AuxParm<Percentage>[] MinRelativeBiomass
        {
            get {
                return minRelativeBiomass;
            }
        }

        //---------------------------------------------------------------------

        public Species.AuxParm<int>     SppFunctionalType {get {return sppFunctionalType;}}
        public Species.AuxParm<int>     NTolerance { get {return nTolerance;}}
        public Species.AuxParm<int>     GDDmin     { get { return gddMin; }}
        public Species.AuxParm<int>     GDDmax     { get { return gddMax; }}
        public Species.AuxParm<int>     MinJanTemp { get { return minJanTemp; }}
        public Species.AuxParm<double>  MaxDrought { get { return maxDrought; }}
        public Species.AuxParm<double>  LeafLongevity {get {return leafLongevity;}}
        //---------------------------------------------------------------------
        /// <summary>
        /// Can the species resprout epicormically following a fire?
        /// </summary>
        public Species.AuxParm<bool>    Epicormic 
        {
            get {
                return epicormic;
            }
            set {
                epicormic = value;
            }
        }

        //---------------------------------------------------------------------
        public Species.AuxParm<double> LeafLignin
        {
            get {
                return leafLignin;
            }
        }
        //---------------------------------------------------------------------
        public Species.AuxParm<double> WoodLignin
        {
            get {
                return woodLignin;
            }
        }
        //---------------------------------------------------------------------
        public Species.AuxParm<double> CoarseRootLignin
        {
            get {
                return coarseRootLignin;
            }
        }
        //---------------------------------------------------------------------
        public Species.AuxParm<double> FineRootLignin
        {
            get {
                return fineRootLignin;
            }
        }
        //---------------------------------------------------------------------

        public Species.AuxParm<double> LeafCN
        {
            get {
                return leafCN;
            }
        }
        //---------------------------------------------------------------------

        public Species.AuxParm<double> WoodCN
        {
            get {
                return woodCN;
            }
        }
        //---------------------------------------------------------------------

        public Species.AuxParm<double> CoarseRootCN
        {
            get {
                return coarseRootCN;
            }
        }
        //---------------------------------------------------------------------

        public Species.AuxParm<double> FoliageLitterCN
        {
            get {
                return foliageLitterCN;
            }
        }
        //---------------------------------------------------------------------

        public Species.AuxParm<double> FineRootLitterCN
        {
            get {
                return fineRootLitterCN;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Definitions of sufficient light probabilities.
        /// </summary>
        public List<ISufficientLight> LightClassProbabilities
        {
            get {
                return sufficientLight;
            }
            set 
            {
                Debug.Assert(sufficientLight.Count != 0);
                sufficientLight = value;
            }
        }
        //---------------------------------------------------------------------

        public Ecoregions.AuxParm<double> PercentClay
        {
            get {
                return percentClay;
            }
        }
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> PercentSand
        {
            get {
                return percentSand;
            }
        }
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<int> SoilDepth
        {
            get {
                return soilDepth;
            }
        }
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> FieldCapacity
        {
            get {
                return fieldCapacity;
            }
        }
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> WiltingPoint
        {
            get {
                return wiltingPoint;
            }
        }
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> StormFlowFraction
        {
            get {
                return stormFlowFraction;
            }
        }
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> BaseFlowFraction
        {
            get {
                return baseFlowFraction;
            }
        }
        
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> Drain
        {
            get {
                return drain;
            }
        }
        //---------------------------------------------------------------------
        /*public Ecoregions.AuxParm<double> MonthlyNDeposition
        {
            get {
                return monthlyNDeposition;
            }
        }*/
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> AtmosNslope
        {
            get {
                return atmosNslope;
            }
        }
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> AtmosNintercept
        {
            get {
                return atmosNintercept;
            }
        }
        
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> Latitude
        {
            get {
                return latitude;
            }
        }
        //---------------------------------------------------------------------
        public Ecoregions.AuxParm<double> InitialSOM1surfC { get { return initSOM1surfC; } }
        public Ecoregions.AuxParm<double> InitialSOM1surfN { get { return initSOM1surfN; } }
        public Ecoregions.AuxParm<double> InitialSOM1soilC { get { return initSOM1soilC; } }
        public Ecoregions.AuxParm<double> InitialSOM1soilN { get { return initSOM1soilN; } }
        public Ecoregions.AuxParm<double> InitialSOM2C { get { return initSOM2C; } }
        public Ecoregions.AuxParm<double> InitialSOM2N { get { return initSOM2N; } }
        public Ecoregions.AuxParm<double> InitialSOM3C { get { return initSOM3C; } }
        public Ecoregions.AuxParm<double> InitialSOM3N { get { return initSOM3N; } }
        public Ecoregions.AuxParm<double> InitialMineralN { get { return initMineralN; } }
        
        //---------------------------------------------------------------------

        public string AgeOnlyDisturbanceParms
        {
            get {
                return ageOnlyDisturbanceParms;
            }
            set {
                //if (value != null) {
                    string path = value;
                    if (path.Trim(null).Length == 0)
                        throw new InputValueException(path,
                                                      "\"{0}\" is not a valid path.",
                                                      path);
                //}
                ageOnlyDisturbanceParms = value;
            }
        }

        //---------------------------------------------------------------------

        public List<Dynamic.ParametersUpdate> DynamicUpdates
        {
            get {
                return dynamicUpdates;
            }
        }
        //---------------------------------------------------------------------

        public void SetMinRelativeBiomass(byte                   shadeClass,
                                          IEcoregion             ecoregion,
                                          InputValue<Percentage> newValue)
        {
            Debug.Assert(1 <= shadeClass && shadeClass <= 5);
            Debug.Assert(ecoregion != null);
            if (newValue != null) {
                if (newValue.Actual < 0.0 || newValue.Actual > 1.0)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between 0% and 100%", newValue.String);
            }
            minRelativeBiomass[shadeClass][ecoregion] = newValue;
        }
        //---------------------------------------------------------------------

        public void SetFunctionalType(ISpecies           species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            sppFunctionalType[species] = CheckBiomassParm(newValue, 0, 100);
        }
        //---------------------------------------------------------------------

        public void SetNTolerance(ISpecies           species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            nTolerance[species] = CheckBiomassParm(newValue, 1, 4);
        }

        //---------------------------------------------------------------------

        public void SetGDDmin(ISpecies           species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            gddMin[species] = CheckBiomassParm(newValue, 1, 4000);
        }
        //---------------------------------------------------------------------

        public void SetGDDmax(ISpecies           species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            gddMax[species] = CheckBiomassParm(newValue, 500, 7000);
        }
        //---------------------------------------------------------------------

        public void SetMinJanTemp(ISpecies           species,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            minJanTemp[species] = CheckBiomassParm(newValue, -60, 20);
        }
        //---------------------------------------------------------------------

        public void SetMaxDrought(ISpecies           species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            maxDrought[species] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetLeafLongevity(ISpecies           species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafLongevity[species] = CheckBiomassParm(newValue, 1.0, 10.0);
        }

        //---------------------------------------------------------------------

        public void SetLeafLignin(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        }
        //---------------------------------------------------------------------

        public void SetWoodLignin(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            woodLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        }
        //---------------------------------------------------------------------

        public void SetCoarseRootLignin(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            coarseRootLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        }
        //---------------------------------------------------------------------

        public void SetFineRootLignin(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            fineRootLignin[species] = CheckBiomassParm(newValue, 0.0, 0.4);
        }
        //---------------------------------------------------------------------

        public void SetLeafCN(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        }
        //---------------------------------------------------------------------

        public void SetWoodCN(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            woodCN[species] = CheckBiomassParm(newValue, 5.0, 600.0);
        }
        //---------------------------------------------------------------------

        public void SetCoarseRootCN(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            coarseRootCN[species] = CheckBiomassParm(newValue, 5.0, 500.0);
        }
        //---------------------------------------------------------------------

        public void SetFoliageLitterCN(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            foliageLitterCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        }
        //---------------------------------------------------------------------

        public void SetFineRootLitterCN(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            fineRootLitterCN[species] = CheckBiomassParm(newValue, 5.0, 100.0);
        }
        //---------------------------------------------------------------------

        public void SetPercentClay(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            percentClay[ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetPercentSand(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            percentSand[ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetSoilDepth(IEcoregion ecoregion, InputValue<int> newValue)
        {
            Debug.Assert(ecoregion != null);
            soilDepth[ecoregion] = CheckBiomassParm(newValue, 1, 200);
        }
        //---------------------------------------------------------------------

        public void SetFieldCapacity(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            fieldCapacity[ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetWiltingPoint(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            wiltingPoint[ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetStormFlowFraction(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            stormFlowFraction[ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetBaseFlowFraction(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            baseFlowFraction[ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetDrain(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            drain[ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }
        //---------------------------------------------------------------------

        public void SetAtmosNslope(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            atmosNslope[ecoregion] = CheckBiomassParm(newValue, 0.0, 2.0);
        }
        //---------------------------------------------------------------------
        public void SetAtmosNintercept(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            atmosNintercept[ecoregion] = CheckBiomassParm(newValue, 0.0, 2.0);
        }
        //---------------------------------------------------------------------
        public void SetLatitude(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            latitude[ecoregion] = CheckBiomassParm(newValue, 0.0, 50.0);
        }
        //---------------------------------------------------------------------
        public void SetInitSOM1surfC(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initSOM1surfC[ecoregion] = CheckBiomassParm(newValue, 0.0, 10000.0);
        }
        //---------------------------------------------------------------------
        public void SetInitSOM1surfN(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initSOM1surfN[ecoregion] = CheckBiomassParm(newValue, 0.0, 500.0);
        }
        //---------------------------------------------------------------------
        public void SetInitSOM1soilC(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initSOM1soilC[ecoregion] = CheckBiomassParm(newValue, 0.0, 10000.0);
        }
        //---------------------------------------------------------------------
        public void SetInitSOM1soilN(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initSOM1soilN[ecoregion] = CheckBiomassParm(newValue, 0.0, 500.0);
        }
        //---------------------------------------------------------------------
        public void SetInitSOM2C(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initSOM2C[ecoregion] = CheckBiomassParm(newValue, 0.0, 10000.0);
        }
        //---------------------------------------------------------------------
        public void SetInitSOM2N(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initSOM2N[ecoregion] = CheckBiomassParm(newValue, 0.0, 500.0);
        }
        //---------------------------------------------------------------------
        public void SetInitSOM3C(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initSOM3C[ecoregion] = CheckBiomassParm(newValue, 0.0, 10000.0);
        }
        //---------------------------------------------------------------------
        public void SetInitSOM3N(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initSOM3N[ecoregion] = CheckBiomassParm(newValue, 0.0, 500.0);
        }
        //---------------------------------------------------------------------
        public void SetInitMineralN(IEcoregion ecoregion, InputValue<double> newValue)
        {
            Debug.Assert(ecoregion != null);
            initMineralN[ecoregion] = CheckBiomassParm(newValue, 0.0, 5000.0);
        }
        //---------------------------------------------------------------------
        //---------------------------------------------------------------------

        public Parameters(Ecoregions.IDataset ecoregionDataset,
                                  Species.IDataset    speciesDataset,
                                  int litterCnt, int functionalCnt)
            : base(ecoregionDataset,
                   speciesDataset)
        {
            this.speciesDataset = speciesDataset;
            this.ecoregionDataset = ecoregionDataset;

            functionalTypes = new FunctionalTypeTable(functionalCnt);
            fireReductionsTable = new FireReductions[6];

            sppFunctionalType       = new Species.AuxParm<int>(speciesDataset);
            nTolerance              = new Species.AuxParm<int>(speciesDataset);
            gddMin                  = new Species.AuxParm<int>(speciesDataset);
            gddMax                  = new Species.AuxParm<int>(speciesDataset);
            minJanTemp              = new Species.AuxParm<int>(speciesDataset);
            maxDrought              = new Species.AuxParm<double>(speciesDataset);
            leafLongevity           = new Species.AuxParm<double>(speciesDataset);
            epicormic               = new Species.AuxParm<bool>(speciesDataset);
            leafLignin              = new Species.AuxParm<double>(speciesDataset);
            woodLignin              = new Species.AuxParm<double>(speciesDataset);
            coarseRootLignin        = new Species.AuxParm<double>(speciesDataset);
            fineRootLignin          = new Species.AuxParm<double>(speciesDataset);
            leafCN                  = new Species.AuxParm<double>(speciesDataset);
            woodCN                  = new Species.AuxParm<double>(speciesDataset);
            coarseRootCN            = new Species.AuxParm<double>(speciesDataset);
            foliageLitterCN         = new Species.AuxParm<double>(speciesDataset);
            fineRootLitterCN        = new Species.AuxParm<double>(speciesDataset);

            minRelativeBiomass = new Ecoregions.AuxParm<Percentage>[6];
            for (byte shadeClass = 1; shadeClass <= 5; shadeClass++) {
                minRelativeBiomass[shadeClass] = new Ecoregions.AuxParm<Percentage>(ecoregionDataset);
            }
            sufficientLight         = new List<ISufficientLight>();//[5];//ListOfEditable<IEditableSufficientLight, ISufficientLight>();

            percentClay             = new Ecoregions.AuxParm<double>(ecoregionDataset);
            percentSand             = new Ecoregions.AuxParm<double>(ecoregionDataset);
            soilDepth               = new Ecoregions.AuxParm<int>(ecoregionDataset);
            fieldCapacity           = new Ecoregions.AuxParm<double>(ecoregionDataset);
            wiltingPoint            = new Ecoregions.AuxParm<double>(ecoregionDataset);
            stormFlowFraction       = new Ecoregions.AuxParm<double>(ecoregionDataset);
            baseFlowFraction        = new Ecoregions.AuxParm<double>(ecoregionDataset);
            drain                   = new Ecoregions.AuxParm<double>(ecoregionDataset);
            //monthlyNDeposition      = new Ecoregions.AuxParm<InputValue<double>>(ecoregionDataset);
            atmosNslope             = new Ecoregions.AuxParm<double>(ecoregionDataset);
            atmosNintercept         = new Ecoregions.AuxParm<double>(ecoregionDataset);
            latitude                = new Ecoregions.AuxParm<double>(ecoregionDataset);

            initSOM1surfC           = new Ecoregions.AuxParm<double>(ecoregionDataset);
            initSOM1surfN           = new Ecoregions.AuxParm<double>(ecoregionDataset);
            initSOM1soilC           = new Ecoregions.AuxParm<double>(ecoregionDataset);
            initSOM1soilN           = new Ecoregions.AuxParm<double>(ecoregionDataset);
            initSOM2C               = new Ecoregions.AuxParm<double>(ecoregionDataset);
            initSOM2N               = new Ecoregions.AuxParm<double>(ecoregionDataset);
            initSOM3C               = new Ecoregions.AuxParm<double>(ecoregionDataset);
            initSOM3N               = new Ecoregions.AuxParm<double>(ecoregionDataset);
            initMineralN            = new Ecoregions.AuxParm<double>(ecoregionDataset);

            
            this.dynamicUpdates = new List<Dynamic.ParametersUpdate>();
            
        }

        //---------------------------------------------------------------------

        private double CheckBiomassParm(InputValue<double> newValue,
                                                    double             minValue,
                                                    double             maxValue)
        {
            if (newValue != null) {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between {1:0.0} and {2:0.0}",
                                                  newValue.String, minValue, maxValue);
            }
            return newValue.Actual;
        }
        //---------------------------------------------------------------------

        private int CheckBiomassParm(InputValue<int> newValue,
                                                    int             minValue,
                                                    int             maxValue)
        {
            if (newValue != null) {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between {1:0.0} and {2:0.0}",
                                                  newValue.String, minValue, maxValue);
            }
            return newValue.Actual;
        }
        //---------------------------------------------------------------------

        private Ecoregions.AuxParm<T> ConvertToActualValues<T>(Ecoregions.AuxParm<InputValue<T>> inputValues)
        {
            Ecoregions.AuxParm<T> actualValues = new Ecoregions.AuxParm<T>(Model.Core.Ecoregions); //ecoregionDataset);
            foreach (IEcoregion ecoregion in Model.Core.Ecoregions)//ecoregionDataset)
                if (inputValues[ecoregion] != null)
                    actualValues[ecoregion] = inputValues[ecoregion].Actual;
            return actualValues;
        }

        //---------------------------------------------------------------------

        private Species.AuxParm<T> ConvertToActualValues<T>(Species.AuxParm<InputValue<T>> inputValues)
        {
            Species.AuxParm<T> actualValues = new Species.AuxParm<T>(Model.Core.Species);//speciesDataset);
            foreach (ISpecies species in Model.Core.Species)//speciesDataset)
                if (inputValues[species] != null)
                    actualValues[species] = inputValues[species].Actual;
            return actualValues;
        }


    }
}
