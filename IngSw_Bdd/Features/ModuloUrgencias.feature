Feature: Modelo de Urgencias
    Esta feature esta relacionada al registro de ingresos de pacientes en la sala 
    de urgencias respetando su nivel de preioridad y el horario de llegada.

Background:
	Given que la siguiente enfermera esta registrada:
		| Id                                   | Name   | LastName |
		| 2D7E2179-254D-4634-9655-56345F27A339 | Melisa | Rios     |

@Escenario1
Scenario: Ingreso del primer paciente a la lista de espera de urgencias
	Given que estan registrados los siguientes pacientes:
		| Id                                   | Cuil          | LastName | Name  | SocialWork    |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Perez    | Maria | Swiss Medical |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Gomez    | Ana   | Galeno        |
	When ingreso a urgencias al siguiente paciente:
		| Id                                   | Cuil          | Informe     | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Tiene gripe | EMERGENCY           |          38 |                  70 |                      15 | 120/80           |
	Then La lista de espera esta ordenada por cuil de la siguiente manera:
		| Cuil          |
		| 20-45625563-9 |
@Escenario2
Scenario: Ingreso de un paciente sin registro previo a la lista de espera de urgencias
	Given que no existe el paciente registrado
	And que estan registradas las siguientes obras sociales:
		| Id                                   | Name          |
		| b4496ccd-1661-48ff-90e9-1432055e0226 | Swiss Medical |
		| abd31a2d-50d9-4b80-87ee-3dfeeae123ba | Galeno        |
	When registro al paciente a urgencias con los siguientes datos:
		| Id                                   | Cuil          | LastName | Name   | Email                   | Phone      | BirthDate  | Street     | Number | Locality   | AffiliateNumber | IdSocialWork                         |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-4 | Auchana  | Leonel | leonel.auchana@mail.com | 3814552211 | 1995-04-12 | San Martín |    452 | San Miguel |         4854166 | b4496ccd-1661-48ff-90e9-1432055e0226 |
	And ingreso a urgencias el nuevo paciente registrado:
		| Cuil          | Informe      | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| 20-45625563-4 | Tiene fiebre | EMERGENCY           |          39 |                  70 |                      15 | 120/80           |
	Then La lista de espera esta ordenada por cuil de la siguiente manera:
		| Cuil          |
		| 20-45625563-4 |
@Escenario3
Scenario: Ingreso de un paciente con datos mandatorios faltantes
	Given que estan registrados los siguientes pacientes:
		| Id                                   | Cuil          | LastName | Name  | SocialWork    |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Perez    | Maria | Swiss Medical |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Gomez    | Ana   | Galeno        |

	When ingreso a urgencias al siguiente paciente:
		| Id                                   | Cuil          | Informe     | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Tiene gripe |                     |          38 |                  70 |                      15 | 120/80           |
	Then se informa la falta del dato mandatario "El nivel de emergencia es requerido."
	And La lista de espera no contendrá el cuil:
		| Cuil          |
		| 20-45625563-9 |
@Escenario4
Scenario: Ingreso de un paciente frecuencia respiratoria negativa
	Given que estan registrados los siguientes pacientes:
		| Id                                   | Cuil          | LastName | Name  | SocialWork    |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Perez    | Maria | Swiss Medical |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Gomez    | Ana   | Galeno        |
	When ingreso a urgencias al siguiente paciente:
		| Id                                   | Cuil          | Informe     | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Tiene gripe | EMERGENCY           |          38 |                  70 |                     -15 | 120/80           |
	Then se informa que la frecuencia respiratorio se cargo de forma incorrecta "La frecuencia respiratoria no puede ser un valor negativo ni igual a cero."
	And La lista de espera no contendrá el cuil:
		| Cuil          |
		| 20-45625563-9 |
@Escenario5
Scenario: Ingreso de un paciente con nivel de emergencia mayor a otro paciente ya en la lista de espera
	Given que estan registrados los siguientes pacientes:
		| Id                                   | Cuil          | LastName | Name   | SocialWork    |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Perez    | Maria  | Swiss Medical |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Gomez    | Ana    | Galeno        |
		| e927b2b1-a878-42dc-8f75-33139bcd7197 | 20-45625777-4 | Auchana  | Leonel | OSDE          |
	And que la lista de espera actual ordenada por nivel es:
		| Id                                   | Cuil          | Informe      | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Fiebre       | EMERGENCY           |          39 |                  71 |                      16 | 120/80           |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Dolor de ojo | URGENCY             |          38 |                  71 |                      16 | 120/80           |
	When ingreso a urgencias al siguiente paciente:
		| Id                                   | Cuil          | Informe   | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| e927b2b1-a878-42dc-8f75-33139bcd7197 | 20-45625777-4 | apuñalada | CRITICAL            |          38 |                  70 |                      15 | 120/80           |
	Then La lista de espera esta ordenada por cuil considerando la prioridad de la siguiente manera:
		| Cuil          |
		| 20-45625777-4 |
		| 20-45625563-3 |
		| 20-45625563-9 |
@Escenario6
Scenario: Ingreso de un paciente con nivel de emergencia menor a otro paciente ya en la lista de espera
	Given que estan registrados los siguientes pacientes:
		| Id                                   | Cuil          | LastName | Name   | SocialWork    |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Perez    | Maria  | Swiss Medical |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Gomez    | Ana    | Galeno        |
		| e927b2b1-a878-42dc-8f75-33139bcd7197 | 20-45625777-4 | Auchana  | Leonel | OSDE          |
	And que la lista de espera actual ordenada por nivel es:
		| Id                                   | Cuil          | Informe | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Fiebre  | EMERGENCY           |          39 |                  71 |                      16 | 120/80           |
	When ingreso a urgencias al siguiente paciente:
		| Id                                   | Cuil          | Informe   | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| e927b2b1-a878-42dc-8f75-33139bcd7197 | 20-45625777-4 | apuñalada | URGENCY             |          38 |                  70 |                      15 | 120/80           |
	Then La lista de espera esta ordenada por cuil considerando la prioridad de la siguiente manera:
		| Cuil          |
		| 20-45625563-3 |
		| 20-45625777-4 |
@Escenario7
Scenario: Ingreso de un paciente con el mismo nivel de emergencia que otro paciente ya en la lista de espera
	Given que estan registrados los siguientes pacientes:
		| Id                                   | Cuil          | LastName | Name   | SocialWork    |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Perez    | Maria  | Swiss Medical |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Gomez    | Ana    | Galeno        |
		| e927b2b1-a878-42dc-8f75-33139bcd7197 | 20-45625777-4 | Auchana  | Leonel | OSDE          |
	And que la lista de espera actual ordenada por nivel es:
		| Id                                   | Cuil          | Informe      | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| 146d0ee4-20f2-451f-b46e-09ef98568bab | 20-45625563-3 | Fiebre       | CRITICAL            |          39 |                  71 |                      16 | 120/80           |
		| 26cf78de-7511-4a94-a466-af8c18927159 | 20-45625563-9 | Dolor de ojo | URGENCY             |          38 |                  71 |                      16 | 120/80           |
	When ingreso a urgencias al siguiente paciente:
		| Id                                   | Cuil          | Informe   | Nivel de Emergencia | Temperatura | Frecuencia Cardiaca | Frecuencia Respiratoria | Tension Arterial |
		| e927b2b1-a878-42dc-8f75-33139bcd7197 | 20-45625777-4 | apuñalada | CRITICAL            |          38 |                  70 |                      15 | 120/80           |
	Then La lista de espera esta ordenada por cuil considerando la prioridad de la siguiente manera:
		| Cuil          |
		| 20-45625563-3 |
		| 20-45625777-4 |
		| 20-45625563-9 |