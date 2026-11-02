import { defineMiddleware } from 'astro:middleware';
import { ADMIN_COOKIE } from './lib/admin/client';

// Every /admin page except the login page needs the admin cookie.
// This only keeps the pages tidy: the real protection is the API, which checks the key on every request.
export const onRequest = defineMiddleware(async (context, next) =>
{
	const path = context.url.pathname;
	const isAdminPage = path === '/admin' || path.startsWith('/admin/');
	const isLoginPage = path === '/admin/login';

	if (isAdminPage && !isLoginPage && !context.cookies.has(ADMIN_COOKIE))
	{
		return context.redirect('/admin/login');
	}
	return next();
});
