using Edu.Wisc.Forest.Flel.Util;

using Landis.Ecoregions;
using Landis.Species;
using Landis.Succession;

using System.Diagnostics;

namespace Landis.Biomass.Succession
{
    /// <summary>
    /// Editable set of parameters for biomass succession.
    /// </summary>
    public class EditableParameters
        : IEditable<IParameters>
    {
        private Ecoregions.IDataset ecoregionDataset;
        private Species.IDataset speciesDataset;

        private InputValue<int> timestep;
        private InputValue<SeedingAlgorithms> seedAlg;
        private Ecoregions.AuxParm<InputValue<Percentage>>[] MinRelativeBiomass;
        private Species.AuxParm<InputValue<double>> leafLongevity;
        private Species.AuxParm<InputValue<double>> WoodyDecayRate;
        private Species.AuxParm<InputValue<double>> mortCurveShapeParm;
        private Species.AuxParm<Ecoregions.AuxParm<InputValue<double>>> establishProbability;
        private Species.AuxParm<Ecoregions.AuxParm<InputValue<int>>> MaxANPP;
        private Species.AuxParm<Ecoregions.AuxParm<InputValue<double>>> leafLitterDecayRate;
        private InputValue<string> ageOnlyDisturbanceParms;

        //---------------------------------------------------------------------

        /// <summary>
        /// Timestep (years)
        /// </summary>
        public InputValue<int> Timestep
        {
            get {
                return timestep;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Timestep must be > or = 0");
                }
                timestep = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Seeding algorithm
        /// </summary>
        public InputValue<SeedingAlgorithms> SeedAlgorithm
        {
            get {
                return seedAlg;
            }

            set {
                seedAlg = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the optional file with the biomass parameters for age-only
        /// disturbances.
        /// </summary>
        public InputValue<string> AgeOnlyDisturbanceParms
        {
            get {
                return ageOnlyDisturbanceParms;
            }

            set {
                if (value != null) {
                    string path = value.Actual;
                    if (path.Trim(null).Length == 0)
                        throw new InputValueException(path,
                                                      "\"{0}\" is not a valid path.",
                                                      path);
                }
                ageOnlyDisturbanceParms = value;
            }
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                object[] parameters = new object[]{ timestep,
                                                    seedAlg };
                foreach (object parameter in parameters)
                    if (parameter == null)
                        return false;
                return true;
            }
        }

        //---------------------------------------------------------------------

        public EditableParameters(Ecoregions.IDataset ecoregionDataset,
                                  Species.IDataset    speciesDataset)
        {
            this.ecoregionDataset = ecoregionDataset;
            this.speciesDataset = speciesDataset;

            MinRelativeBiomass = new Ecoregions.AuxParm<InputValue<Percentage>>[6];
            for (byte shadeClass = 1; shadeClass <= 5; shadeClass++) {
                MinRelativeBiomass[shadeClass] = new Ecoregions.AuxParm<InputValue<Percentage>>(ecoregionDataset);
            }

            leafLongevity = new Species.AuxParm<InputValue<double>>(speciesDataset);
            WoodyDecayRate = new Species.AuxParm<InputValue<double>>(speciesDataset);
            mortCurveShapeParm = new Species.AuxParm<InputValue<double>>(speciesDataset);

            establishProbability = CreateSpeciesEcoregionParm<double>();
            MaxANPP = CreateSpeciesEcoregionParm<int>();
            leafLitterDecayRate = CreateSpeciesEcoregionParm<double>();
        }

        //---------------------------------------------------------------------

        private Species.AuxParm<Ecoregions.AuxParm<InputValue<T>>> CreateSpeciesEcoregionParm<T>()
        {
            Species.AuxParm<Ecoregions.AuxParm<InputValue<T>>> newParm;
            newParm = new Species.AuxParm<Ecoregions.AuxParm<InputValue<T>>>(speciesDataset);
            foreach (ISpecies species in speciesDataset) {
                newParm[species] = new Ecoregions.AuxParm<InputValue<T>>(ecoregionDataset);
            }
            return newParm;
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
            MinRelativeBiomass[shadeClass][ecoregion] = newValue;
        }

        //---------------------------------------------------------------------

        public void SetLeafLongevity(ISpecies           species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            leafLongevity[species] = CheckBiomassParm(newValue, 1.0, 10.0);
        }

        //---------------------------------------------------------------------

        public void SetWoodyDecayRate(ISpecies           species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            WoodyDecayRate[species] = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        //---------------------------------------------------------------------

        public void SetMortCurveShapeParm(ISpecies           species,
                                          InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            mortCurveShapeParm[species] = CheckBiomassParm(newValue, 5.0, 25.0);
        }

        //---------------------------------------------------------------------

        private InputValue<double> CheckBiomassParm(InputValue<double> newValue,
                                                    double             minValue,
                                                    double             maxValue)
        {
            if (newValue != null) {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between {1:0.0} and {2:0.0}",
                                                  newValue.String, minValue, maxValue);
            }
            return newValue;
        }

        //---------------------------------------------------------------------

        public void SetEstablishProbability(ISpecies           species,
                                            IEcoregion         ecoregion,
                                            InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            Debug.Assert(ecoregion != null);
            establishProbability[species][ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        //---------------------------------------------------------------------

        public void SetMaxANPP(ISpecies        species,
                                     IEcoregion      ecoregion,
                                     InputValue<int> newValue)
        {
            Debug.Assert(species != null);
            Debug.Assert(ecoregion != null);
            if (newValue != null) {
                if (newValue.Actual < 0 || newValue.Actual > 10000)
                    throw new InputValueException(newValue.String,
                                                  "{0} is not between 0 and 10,000",
                                                  newValue.String);
            }
            MaxANPP[species][ecoregion] = newValue;
        }

        //---------------------------------------------------------------------

        public void SetLeafLitterDecayRate(ISpecies           species,
                                           IEcoregion         ecoregion,
                                           InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            Debug.Assert(ecoregion != null);
            leafLitterDecayRate[species][ecoregion] = CheckBiomassParm(newValue, 0.0, 1.0);
        }

        //---------------------------------------------------------------------

        public IParameters GetComplete()
        {
            if (this.IsComplete) {
                Ecoregions.AuxParm<Percentage>[] maxRelBiomass = new Ecoregions.AuxParm<Percentage>[6];
                for (byte shadeClass = 1; shadeClass <= 5; shadeClass++) {
                    maxRelBiomass[shadeClass] = ConvertToActualValues(MinRelativeBiomass[shadeClass]);
                }

                string ageOnlyDistParmFile = null;
                if (ageOnlyDisturbanceParms != null)
                    ageOnlyDistParmFile = ageOnlyDisturbanceParms.Actual;

                return new Parameters(timestep.Actual,
                                      seedAlg.Actual,
                                      maxRelBiomass,
                                      ConvertToActualValues(leafLongevity),
                                      ConvertToActualValues(WoodyDecayRate),
                                      ConvertToActualValues(mortCurveShapeParm),
                                      ConvertToActualValues(establishProbability),
                                      ConvertToActualValues(MaxANPP),
                                      ConvertToActualValues(leafLitterDecayRate),
                                      ageOnlyDistParmFile);
            }
            else
                return null;
        }

        //---------------------------------------------------------------------

        private Ecoregions.AuxParm<T> ConvertToActualValues<T>(Ecoregions.AuxParm<InputValue<T>> inputValues)
        {
            Ecoregions.AuxParm<T> actualValues = new Ecoregions.AuxParm<T>(ecoregionDataset);
            foreach (IEcoregion ecoregion in ecoregionDataset)
                if (inputValues[ecoregion] != null)
                    actualValues[ecoregion] = inputValues[ecoregion].Actual;
            return actualValues;
        }

        //---------------------------------------------------------------------

        private Species.AuxParm<T> ConvertToActualValues<T>(Species.AuxParm<InputValue<T>> inputValues)
        {
            Species.AuxParm<T> actualValues = new Species.AuxParm<T>(speciesDataset);
            foreach (ISpecies species in speciesDataset)
                if (inputValues[species] != null)
                    actualValues[species] = inputValues[species].Actual;
            return actualValues;
        }

        //---------------------------------------------------------------------

        private Species.AuxParm<Ecoregions.AuxParm<T>> ConvertToActualValues<T>(Species.AuxParm<Ecoregions.AuxParm<InputValue<T>>> inputValues)
        {
            Species.AuxParm<Ecoregions.AuxParm<T>> actualValues = new Species.AuxParm<Ecoregions.AuxParm<T>>(speciesDataset);
            foreach (ISpecies species in speciesDataset)
                actualValues[species] = ConvertToActualValues(inputValues[species]);
            return actualValues;
        }
    }
}
