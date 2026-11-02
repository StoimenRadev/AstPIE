import { t } from './t';
import type { SimulationLabels } from '../components/SimulationPlayer';

// All texts the simulation player needs, in the current language
export function simulationLabels(): SimulationLabels
{
	return {
		start: t('simulation.start'),
		narrowScreen: t('simulation.narrowScreen'),
		loading: t('simulation.loading'),
		starting: t('simulation.starting'),
		error: t('simulation.error'),
		retry: t('simulation.retry'),
		expand: t('simulation.expand'),
		minimise: t('simulation.minimise')
	};
}
