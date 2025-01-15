/* FILE HEADER
 * AUTHOR: Chase Morgan | CREATED: 01/09/2025
 * UPDATED: 01/09/2025 | BY: Chase Morgan  | COMMENTS: Added interface/namespace
 * FILE DESCRIPTION: Client class for any classes that want to implement the Strategy Pattern methods
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace ChaseMorgan.Strategy
{
    public class Client : MonoBehaviour
    {
        //FIELDS

        [SerializeField, Tooltip("Allows the client to perform multiple strategies at once. " +
            "WARNING!!! Strategies must be manually cancelled if enabled!")] 
        protected bool m_canDoMultipleStrategies = false;
        [SerializeField, Tooltip("The maximum allowed strategies this client can perform at any given time. " +
            "Leave at 0 or below to have unlimited. " +
            "If a strategy is applied that will overflow the bottom strategy will automatically be disabled!")]
        protected int m_maxStrategies = -1;

        protected List<IStrategy> m_strategies = new List<IStrategy>();
        protected List<IStrategy> m_activeStrategies = new List<IStrategy>();

        //PROPERTIES

        public bool CanDoMultipleStrategies => m_canDoMultipleStrategies;

        //METHODS

        /// <summary>
        /// Attempts to apply a strategy to the current client
        /// </summary>
        /// <param name="strategy">The strategy REFERENCE</param>
        public virtual void ApplyStrategy(IStrategy strategy, UnityAction callback = null)
        {
            if (m_activeStrategies.Contains(strategy)) //If this strategy is already active, ignore...
            {
                return;
            }

            if (!m_strategies.Contains(strategy)) //Add the strategy if unrecognized
            {
                m_strategies.Add(strategy);
            }

            while (m_activeStrategies.Count + 1 >= m_maxStrategies && m_maxStrategies > 0) //Just in case multiple strategies slipped passed max without checking
            {
                try
                {
                    IStrategy lastStrategy = m_activeStrategies.Last();
                    lastStrategy.Disable();
                    m_activeStrategies.RemoveAt(m_activeStrategies.IndexOf(lastStrategy));
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Tried to remove strategy on " + name + ", but an exception was thrown! " + e.ToString() + "\nCanceling loop!");
                }
            }

            if (!m_canDoMultipleStrategies)
            {
                m_activeStrategies.ForEach((strat) =>
                {
                    strat.Disable();
                });

                m_activeStrategies.Clear();
            }

            m_activeStrategies.Add(strategy);
            strategy.Execute(this, callback);
        }

        /// <summary>
        /// Attempts to apply a strategy to the current client
        /// </summary>
        /// <typeparam name="T">The strategy TYPE</typeparam>
        /// <returns>true if successful</returns>
        public virtual bool ApplyStrategy<T>(UnityAction callback = null) where T : IStrategy //Generic so we can just apply strategies by type instead of instance
        {
            IStrategy strategy = m_strategies.OfType<T>().FirstOrDefault();

            if (m_activeStrategies.OfType<T>().FirstOrDefault() != null) //If this strategy is already active, ignore...
            {
                return false;
            }

            if (strategy == null) //can't apply a null strategy, return false
            {
                Debug.LogWarning("Attempted to apply a null strategy!");
                return false;
            }

            while (m_activeStrategies.Count + 1 >= m_maxStrategies && m_maxStrategies > 0) //Just in case multiple strategies slipped passed max without checking
            {
                try
                {
                    IStrategy lastStrategy = m_activeStrategies.Last();
                    lastStrategy.Disable();
                    m_activeStrategies.RemoveAt(m_activeStrategies.IndexOf(lastStrategy));
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Tried to remove strategy on " + name + ", but an exception was thrown! " + e.ToString() + "\nCanceling loop!");
                }
            }

            if (!m_canDoMultipleStrategies)
            {
                m_activeStrategies.ForEach((strat) =>
                {
                    strat.Disable();
                });

                m_activeStrategies.Clear();
            }

            m_activeStrategies.Add(strategy);
            strategy.Execute(this, callback);

            return true;
        }
        /// <summary>
        /// Attempts to apply a strategy to the current client
        /// </summary>
        /// <typeparam name="T">The Strategy TYPE</typeparam>
        /// <param name="strategy">The strategy REFERENCE</param>
        /// <returns>true if successful</returns>

        public virtual bool ApplyStrategy<T>(out T strategy, UnityAction callback = null) where T : IStrategy //Generic so we can just apply strategies by type instead of instance
        {
            strategy = m_strategies.OfType<T>().FirstOrDefault();

            if (m_activeStrategies.OfType<T>().FirstOrDefault() != null) //If this strategy is already active, ignore...
            {
                return false;
            }

            if (strategy == null) //can't apply a null strategy, return false
            {
                Debug.LogWarning("Attempted to apply a null strategy!");
                return false;
            }

            while (m_activeStrategies.Count + 1 >= m_maxStrategies && m_maxStrategies > 0) //Just in case multiple strategies slipped passed max without checking
            {
                try
                {
                    IStrategy lastStrategy = m_activeStrategies.Last();
                    lastStrategy.Disable();
                    m_activeStrategies.RemoveAt(m_activeStrategies.IndexOf(lastStrategy));
                }
                catch(Exception e)
                {
                    Debug.LogWarning("Tried to remove strategy on " + name + ", but an exception was thrown! " + e.ToString() + "\nCanceling loop!");
                }
            }

            if (!m_canDoMultipleStrategies)
            {
                m_activeStrategies.ForEach((strat) =>
                {
                    strat.Disable();
                });

                m_activeStrategies.Clear();
            }

            m_activeStrategies.Add(strategy);
            strategy.Execute(this, callback);

            return true;
        }

        public virtual void DisableStrategy<T>() where T : IStrategy
        {
            IStrategy strategy = m_activeStrategies.OfType<T>().FirstOrDefault(); //This tries to find the active strategy by type instead of reference (default is null)
            if (strategy != null)
            {
                m_activeStrategies.Remove(strategy);
                strategy.Disable();
            }
            else
            {
                Debug.LogWarning("Tried to disable a strategy that was either unknown or inactive!\nIgnoring...");
            }
        }

        public virtual void DisableAllStrategies()
        {
            m_activeStrategies.ForEach((strat) =>
            {
                strat.Disable();
            });

            m_activeStrategies.Clear();
        }

        public virtual IStrategy TryGetStrategy<T>() where T : IStrategy
        {
            IStrategy strategy = m_strategies.OfType<T>().FirstOrDefault();

            return strategy;
        }
    }
}
