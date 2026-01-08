import { inject } from "@angular/core";
import { Router } from "@angular/router";
import type {
  CanActivateFn,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from "@angular/router";
import Keycloak from "keycloak-js";
import { logError } from "@orange-car-rental/util";

/**
 * Authentication guard for protecting routes
 * Redirects to /login with returnUrl instead of Keycloak redirect
 */
export const authGuard: CanActivateFn = async (
  _route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const keycloak = inject(Keycloak);
  const router = inject(Router);

  try {
    const authenticated = keycloak.authenticated ?? false;

    if (!authenticated) {
      // Redirect to custom login page with return URL
      router.navigate(["/login"], {
        queryParams: { returnUrl: state.url },
      });
      return false;
    }

    return true;
  } catch (error) {
    logError("AuthGuard", "Auth guard error", error);
    router.navigate(["/login"]);
    return false;
  }
};

/**
 * Agent role guard for call center routes
 * Only allows users with call-center-agent, call-center-supervisor, or admin roles
 */
export const agentGuard: CanActivateFn = async (
  _route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const keycloak = inject(Keycloak);
  const router = inject(Router);

  try {
    const authenticated = keycloak.authenticated ?? false;

    if (!authenticated) {
      // Redirect to login with return URL
      router.navigate(["/login"], {
        queryParams: { returnUrl: state.url },
      });
      return false;
    }

    // Check for agent roles
    const roles = keycloak.realmAccess?.roles ?? [];
    const agentRoles = ["call-center-agent", "call-center-supervisor", "admin"];
    const hasAgentRole = agentRoles.some((role) => roles.includes(role));

    if (!hasAgentRole) {
      // Non-agents are redirected to home
      router.navigate(["/"]);
      return false;
    }

    return true;
  } catch (error) {
    logError("AgentGuard", "Agent guard error", error);
    router.navigate(["/"]);
    return false;
  }
};
