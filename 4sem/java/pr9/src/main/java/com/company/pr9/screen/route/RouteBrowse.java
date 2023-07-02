package com.company.pr9.screen.route;

import io.jmix.ui.screen.*;
import com.company.pr9.entity.Route;

@UiController("tr_Route.browse")
@UiDescriptor("route-browse.xml")
@LookupComponent("routesTable")
public class RouteBrowse extends StandardLookup<Route> {
}