Documentação do Projeto: Minha Saúde Feminina 

O Minha Saúde Feminina é um ecossistema digital completo e acolhedor, voltado para a gestão da saúde da mulher. O seu principal objetivo é acompanhar a utilizadora em todas as fases da vida, combinando ferramentas práticas de monitorização, educação em saúde e suporte, garantindo um acompanhamento seguro, anónimo e altamente personalizado.

Tecnologias e Arquitetura do Sistema

O aplicativo foi concebido com tecnologias modernas para garantir alta performance, segurança e uma experiência fluida nos dispositivos das utilizadoras:

Framework Principal: .NET MAUI (Multi-platform App UI).

Linguagem de Programação: C# (Ecossistema .NET).

Foco: Dispositivos móveis (nativamente compilado para Android e iOS) a partir de uma única base de código.

Padrão de Arquitetura: MVVM (Model-View-ViewModel).

Model: Gere os dados e a lógica de negócio (ex: Entidades da base de dados).

View: A interface visual (telas desenvolvidas em XAML/C#), que é totalmente desacoplada da lógica.

ViewModel: Faz a ponte entre a View e o Model, utilizando Data Binding para reatividade, o que garante um código muito mais limpo, testável e preparado para atualizações futuras.

Base de Dados: PostgreSQL (Modelo Relacional robusto, ideal para a integridade de dados sensíveis de saúde).

ORM (Object-Relational Mapping): Entity Framework Core, facilitando a manipulação e consulta dos dados em C#.

Comunicação: Arquitetura baseada em API REST para a integração entre a aplicação móvel (Front-end) e os servidores (Back-end).

Estruturação do Conteúdo e Eixos Temáticos

Para oferecer um suporte verdadeiramente completo, a aplicação está estruturada em cinco pilares fundamentais de cuidado e adapta-se a diferentes fases da vida.

Módulos por Fase da Vida

O sistema personaliza o conteúdo consoante o momento da utilizadora:

Adolescência

Fase Adulta

Tentantes (Foco em engravidar)

Gestação e Pós-parto

Climatério e Menopausa

Senescência (Saúde na terceira idade)

Condições Crônicas (Suporte contínuo)

Os 5 Pilares Fundamentais

Monitorização do Ciclo: Calendário interativo para o registo do ciclo menstrual, intensidade do fluxo, sintomas diários e projeção inteligente de padrões futuros (janela fértil e próxima menstruação).

Educação e Autoconhecimento: Biblioteca com artigos acessíveis sobre o corpo, saúde íntima, contraceção e prevenção de infeções, curada por especialistas.

Suporte Anónimo e Acolhedor: Um chat seguro onde a mulher pode enviar dúvidas de forma anónima (ex: responder à dúvida clássica "É normal isto?"), desmistificando sintomas e orientando-a a procurar ajuda médica quando necessário.

Prevenção e Rotina de Saúde: Central de notificações com alertas inteligentes para a toma da pílula, marcação de exames preventivos (Papanicolau, Mamografia) e vacinação (ex: HPV).

Saúde Integral e Suporte para Consultas: Diário para o mapeamento de emoções e queixas, preparando a paciente para ir à sua Unidade Básica de Saúde (UBS) com um histórico clínico bem organizado.

Estrutura e Modelação de Dados (PostgreSQL)

A arquitetura de dados relacional foi pensada para garantir conformidade com as leis de proteção de dados (como a LGPD) e a rastreabilidade das informações de saúde.

Pessoa: Armazena o id_pessoa (UUID), nome_completo, data_nascimento (para calcular a idade e direcionar rastreios como o Papanicolau aos 25 anos), email e senha_hash (segurança).

FaseVida: Tabela de domínio para categorizar as etapas (ex: Menarca, Idade Fértil).

ArtigoEducacional: Guarda os conteúdos, dicas de autocuidado e a categoria_tematica.

RegistroCiclo: Gere as datas de data_inicio, data_fim, intensidade_fluxo e a projeção média.

DiarioSintomas: Diário clínico com o tipo_sintoma, nivel_intensidade (1 a 5), e uma flag crucial aciona_alerta_ubs (para avisar o médico em caso de sintomas graves).

AlertaSaude: Gere os lembretes (tipo_alerta, data_agendada) para garantir o cumprimento da rotina clínica.

Interface e Experiência de Utilizador (UX/UI)

O design da aplicação foi desenvolvido com um foco imenso na empatia, clareza e facilidade de uso:

Identidade Visual: Paleta de cores baseada em tons de Roxo e Rosa suave, com elementos visuais de cantos arredondados, transmitindo tranquilidade e segurança.

Navegação Principal (TabBar): * Hoje: O Dashboard principal com o status atual do ciclo, dicas do dia e resumo de consultas.

Ciclo: O calendário visual interativo para registo e visualização do histórico.

Botão Flutuante Central (+): Acesso rápido e de emergência ao Suporte Anónimo (Chat).

Conteúdos: Acesso aos artigos e à educação sexual.

Perfil: Gestão de dados pessoais, partilha de informações para pesquisa (opt-in) e configuração de notificações.

Notificações Laterais: Um painel deslizante (Drawer) intuitivo para verificar avisos pendentes sem sair da navegação principal.
