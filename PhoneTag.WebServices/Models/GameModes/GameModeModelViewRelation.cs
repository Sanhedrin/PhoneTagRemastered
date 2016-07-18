using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Views.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PhoneTag.SharedCodebase.Models.GameModes
{
    public static class GameModeModelViewRelation
    {
        private static readonly Dictionary<Type, Type> sr_ViewForModel = new Dictionary<Type, Type>()
        {
            { typeof(TDMGameMode), typeof(TDMGameModeView) },
            { typeof(VIPGameMode), typeof(VIPGameModeView) }
        };
        
        //Makes sure that all the types entered in the above dictionary are types of game mode objects.
        static GameModeModelViewRelation()
        {
            foreach (Type type in sr_ViewForModel.Values)
            {
                if (!type.GetTypeInfo().IsSubclassOf(typeof(GameModeView)))
                {
                    System.Diagnostics.Debug.WriteLine("Game mode type list contains an invalid mode object");
                    throw new Exception("Game mode type list contains an invalid mode object");
                }
            }

            foreach (Type type in sr_ViewForModel.Keys)
            {
                if (!type.GetTypeInfo().IsSubclassOf(typeof(GameMode)))
                {
                    System.Diagnostics.Debug.WriteLine("Game mode type list contains an invalid mode object");
                    throw new Exception("Game mode type list contains an invalid mode object");
                }
            }
        }

        /// <summary>
        /// Gets the type used as a view for the given model of a game mode.
        /// </summary>
        public static Type GetViewTypeForModel(Type i_ModelType)
        {
            if (!sr_ViewForModel.Keys.Contains(i_ModelType))
            {
                throw new ArgumentException("Given type is not of a registered game mode model.");
            }

            return sr_ViewForModel[i_ModelType];
        }

        /// <summary>
        /// Gets the type used as a model for the given view of a game mode.
        /// </summary>
        public static Type GetModelTypeForView(Type i_ViewType)
        {
            Type modelType = null;

            if (!sr_ViewForModel.Values.Contains(i_ViewType))
            {
                throw new ArgumentException("Given type is not of a registered game mode model.");
            }

            foreach(KeyValuePair<Type, Type> modelViewPair in sr_ViewForModel)
            {
                if (modelViewPair.Value.Equals(i_ViewType))
                {
                    modelType = modelViewPair.Key;
                }
            }

            return modelType;
        }
    }
}