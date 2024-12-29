import { HeroSection } from "@/components/Customization/HeroSection"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { PropsWithChildren } from "react"

interface ManageEventDashboardLayoutProps {
  title: string
}

export const ManageEventDashboardLayout = (props: PropsWithChildren<ManageEventDashboardLayoutProps>) => {
  return (
    <div className="tailwind">
        <HeroSection Title='Manage Event' Description='We can’t wait to see the results.' />
        <section className="">
          <div className="container">
              <div className='grid grid-cols-12 py-5 gap-8'>
                  <div className='col-span-4 '>
                      <Card>
                        <CardContent>
                          <h2 className='text-primary mt-4 mb-5'>Manage Event</h2>
                          <p>
                              This page allows you to create a new event or edit an existing event. You can set the
                              name, time, and location for the event, and then request services from TrashMob.eco
                              Partners.
                          </p>
                        </CardContent>
                      </Card>
                  </div>
                  <div className="col-span-8">
                      <Card>
                        <CardHeader>
                          <CardTitle>{props.title}</CardTitle>
                        </CardHeader>
                        <CardContent>
                          {props.children}
                        </CardContent>
                      </Card>
                  </div>
              </div>
          </div>
        </section>
    </div>
  )
}