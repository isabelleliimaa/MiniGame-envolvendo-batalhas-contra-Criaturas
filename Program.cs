using System;
using System.Threading;

// ============================================================
// CLASSE BASE: representa qualquer criatura do jogo
// ============================================================
public abstract class Criatura
{
    // Gerador de números aleatórios COMPARTILHADO por todo o jogo.
    // (Criar vários "new Random()" em sequência pode gerar os mesmos números.)
    public static readonly Random Rng = new Random();

    public string Nome;
    public int Vida;
    public int VidaMaxima;
    public int Ataque;
    public int Defesa;

    // Propriedade calculada: a criatura está viva enquanto tiver vida > 0
    public bool EstaViva => Vida > 0;

    protected Criatura(string nome, int vida, int ataque, int defesa)
    {
        Nome = nome;
        Vida = vida;
        VidaMaxima = vida;
        Ataque = ataque;
        Defesa = defesa;
    }

    // Cada criatura implementa seu próprio jeito de atacar
    public abstract void Atacar(Criatura alvo);

    // Aplica dano e impede que a vida fique negativa
    public virtual void ReceberDano(int dano)
    {
        if (dano > 0)
        {
            Vida -= dano;
            if (Vida < 0) Vida = 0;
            Console.WriteLine($"{Nome} recebeu {dano} de dano! Vida restante: {Vida}");
        }
    }

    // Recupera 30% da vida máxima (sem ultrapassar o limite)
    public virtual void Descansar()
    {
        Vida = Math.Min(VidaMaxima, Vida + (int)(VidaMaxima * 0.3));
        Console.WriteLine($"{Nome} descansou e recuperou vida! Vida: {Vida}");
    }
}

// ============================================================
// JOGADOR: o Guerreiro controlado pelo usuário
// ============================================================
public class Guerreiro : Criatura
{
    public string Arma;
    public int Sal = 3;
    public int AguaBenta = 2;

    // Controle de fuga (CORREÇÃO: antes a fuga zerava a vida e contava como derrota)
    public bool Fugiu;           // true se o jogador escapou da batalha atual
    public bool PodeFugir = true; // false em batalhas contra chefes

    public Guerreiro(string nome, string arma = "Crucifixo") : base(nome, 100, 25, 15)
    {
        Arma = arma;
    }

    // Mostra o menu de ações e executa a escolha do jogador
    public override void Atacar(Criatura alvo)
    {
        Console.WriteLine($"1. Atacar com {Arma}");
        Console.WriteLine($"2. Usar Sal Grosso ({Sal} restantes)");
        Console.WriteLine($"3. Usar Água Benta ({AguaBenta} restantes)");
        Console.WriteLine($"4. Beber Água Benta para curar (+50 HP)");
        if (PodeFugir) Console.WriteLine("5. Tentar fugir"); // só aparece se for possível fugir

        Console.Write("Escolha sua ação: ");
        string escolha = Console.ReadLine();

        switch (escolha)
        {
            case "1":
                AtaqueNormal(alvo);
                break;
            case "2":
                if (Sal > 0)
                {
                    UsarSal(alvo);
                    Sal--;
                }
                else
                {
                    Console.WriteLine("Você não tem mais Sal Grosso!");
                    AtaqueNormal(alvo);
                }
                break;
            case "3":
                if (AguaBenta > 0)
                {
                    UsarAguaBenta(alvo);
                    AguaBenta--;
                }
                else
                {
                    Console.WriteLine("Você não tem mais Água Benta!");
                    AtaqueNormal(alvo);
                }
                break;
            case "4":
                if (AguaBenta > 0)
                {
                    BeberAguaBenta();
                    AguaBenta--;
                }
                else
                {
                    Console.WriteLine("Você não tem Água Benta para beber!");
                    AtaqueNormal(alvo);
                }
                break;
            case "5" when PodeFugir: // só aceita a opção 5 quando a fuga é permitida
                TentarFugir();
                break;
            default:
                Console.WriteLine("Ação inválida! Atacando normalmente...");
                AtaqueNormal(alvo);
                break;
        }
    }

    // Ataque básico: 10% de errar, 20% de crítico (dano dobrado)
    private void AtaqueNormal(Criatura alvo)
    {
        if (Rng.NextDouble() < 0.1)
        {
            Console.WriteLine($"{Nome} errou o ataque com {Arma}!");
            return;
        }

        int danoBase = Ataque;
        bool critico = Rng.NextDouble() < 0.2;

        if (critico)
        {
            danoBase *= 2;
            Console.WriteLine($"{Nome} acertou um golpe crítico com {Arma}!");
        }

        // A defesa do alvo reduz metade do seu valor do dano (mínimo 1)
        int danoFinal = Math.Max(1, danoBase - (alvo.Defesa / 2));
        Console.WriteLine($"{Nome} ataca {alvo.Nome} com {Arma}!");
        alvo.ReceberDano(danoFinal);
    }

    // Sal Grosso: dano fixo de 40, ignora defesa
    private void UsarSal(Criatura alvo)
    {
        Console.WriteLine($"{Nome} joga Sal Grosso em {alvo.Nome}!");
        int dano = 40;
        Console.WriteLine($"O sal queima a criatura! {dano} de dano!");
        alvo.ReceberDano(dano);
    }

    // Água Benta (arremessada): dano fixo de 60, ignora defesa
    private void UsarAguaBenta(Criatura alvo)
    {
        Console.WriteLine($"{Nome} asperge Água Benta em {alvo.Nome}!");
        int dano = 60;
        Console.WriteLine($"A água santa causa dano massivo! {dano} de dano!");
        alvo.ReceberDano(dano);
    }

    // Água Benta (bebida): cura 50 de vida
    private void BeberAguaBenta()
    {
        int cura = 50;
        Vida = Math.Min(VidaMaxima, Vida + cura);
        Console.WriteLine($"{Nome} bebe Água Benta e recupera {cura} de vida!");
        Console.WriteLine($"Vida atual: {Vida}/{VidaMaxima}");
    }

    // 40% de chance de fugir.
    // CORREÇÃO 1: antes zerava a vida (Vida = 0), o que contava como morte -> GAME OVER.
    // CORREÇÃO 2: antes, ao falhar, o inimigo atacava duas vezes no mesmo turno.
    private void TentarFugir()
    {
        if (Rng.NextDouble() < 0.4)
        {
            Console.WriteLine("Você conseguiu fugir da batalha!");
            Fugiu = true;
        }
        else
        {
            // O inimigo já ataca normalmente no turno dele, então só avisamos aqui
            Console.WriteLine("Não foi possível fugir! A criatura te bloqueia!");
        }
    }
}

// ============================================================
// INIMIGOS
// ============================================================

// Fantasma: 25% de chance de o ataque atravessar o alvo (sem dano)
public class Fantasma : Criatura
{
    public Fantasma(string nome) : base(nome, 60, 18, 8) { }

    public override void Atacar(Criatura alvo)
    {
        if (Rng.NextDouble() < 0.25)
        {
            Console.WriteLine($"{Nome} passa através de {alvo.Nome} sem causar dano!");
            return;
        }

        // Fantasmas ignoram boa parte da defesa (só 1/4 conta)
        int danoFinal = Math.Max(1, Ataque - (alvo.Defesa / 4));
        Console.WriteLine($"{Nome} assombra {alvo.Nome} com um toque gelado!");
        alvo.ReceberDano(danoFinal);
    }
}

// Demônio: 15% de chance de usar chamas infernais (dano fixo maior)
public class Demonio : Criatura
{
    public Demonio(string nome) : base(nome, 120, 35, 20) { }

    public override void Atacar(Criatura alvo)
    {
        if (Rng.NextDouble() < 0.15)
        {
            Console.WriteLine($"{Nome} invoca chamas infernais em {alvo.Nome}!");
            alvo.ReceberDano(Ataque + 10);
        }
        else
        {
            int danoFinal = Math.Max(1, Ataque - (alvo.Defesa / 2));
            Console.WriteLine($"{Nome} ataca {alvo.Nome} com garras demoníacas!");
            alvo.ReceberDano(danoFinal);
        }
    }
}

// Chefe 1: um Demônio com status próprios
public class Ceifeiro : Demonio
{
    public Ceifeiro() : base("O Ceifeiro")
    {
        Vida = 100;
        VidaMaxima = 100;
        Ataque = 30;
        Defesa = 15;
    }

    public override void Atacar(Criatura alvo)
    {
        Console.WriteLine("A foice do Ceifeiro brilha com uma luz sinistra...");
        base.Atacar(alvo); // reaproveita o ataque do Demônio
    }
}

// Chefe final: um Fantasma com status próprios
public class Retorcido : Fantasma
{
    public Retorcido() : base("O Retorcido")
    {
        Vida = 130;
        VidaMaxima = 130;
        Ataque = 30;
        Defesa = 15;
    }

    public override void Atacar(Criatura alvo)
    {
        Console.WriteLine("O Retorcido emite um grito ensurdecedor...");
        base.Atacar(alvo); // reaproveita o ataque do Fantasma
    }
}

// ============================================================
// SISTEMA DE BATALHA
// ============================================================
public class Batalha
{
    // Retorna true se o jogador sobreviveu (vitória OU fuga)
    // e false se o jogador morreu.
    public bool Iniciar(Guerreiro jogador, Criatura inimigo, string recompensaTipo = "")
    {
        Console.WriteLine($"BATALHA CONTRA: {inimigo.Nome}");
        Console.WriteLine(new string('=', 30));

        // Prepara o estado de fuga: não se pode fugir de chefes
        jogador.Fugiu = false;
        jogador.PodeFugir = recompensaTipo != "chefe";

        int round = 1;

        while (jogador.EstaViva && inimigo.EstaViva)
        {
            Console.WriteLine($"\n--- ROUND {round} ---");
            // CORREÇÃO: antes mostrava "/100" fixo; agora usa a vida máxima real
            Console.WriteLine($"{jogador.Nome}: {jogador.Vida}/{jogador.VidaMaxima} HP");
            Console.WriteLine($"{inimigo.Nome}: {inimigo.Vida}/{inimigo.VidaMaxima} HP");
            Console.WriteLine($"Sal: {jogador.Sal} | Água Benta: {jogador.AguaBenta}");
            Console.WriteLine(new string('-', 20));

            // Turno do jogador
            jogador.Atacar(inimigo);

            // Sai do combate se o inimigo morreu ou se o jogador fugiu
            if (!inimigo.EstaViva || jogador.Fugiu) break;

            // Turno do inimigo
            Console.WriteLine($"Turno de {inimigo.Nome}!");
            inimigo.Atacar(jogador);

            round++;
            Thread.Sleep(1000); // pausa dramática entre rounds
        }

        // Resultado da batalha
        if (!jogador.EstaViva)
        {
            Console.WriteLine($"Você foi derrotado por {inimigo.Nome}...");
            return false;
        }

        if (jogador.Fugiu)
        {
            // Fugiu: segue a história, mas sem recompensa
            Console.WriteLine($"Você escapou de {inimigo.Nome} e segue seu caminho...");
            return true;
        }

        Console.WriteLine($"Você derrotou {inimigo.Nome}!");
        DarRecompensa(jogador, recompensaTipo); // itens por vencer
        jogador.Descansar();
        return true;
    }

    // Dá itens ao jogador conforme o tipo de inimigo derrotado
    private void DarRecompensa(Guerreiro jogador, string tipo)
    {
        switch (tipo)
        {
            case "fantasma":
                int salGanho = Criatura.Rng.Next(1, 3); // 1 ou 2
                jogador.Sal += salGanho;
                Console.WriteLine($"Recompensa: +{salGanho} Sal Grosso!");
                break;

            case "demonio":
                // CORREÇÃO: Next(1, 2) sempre retornava 1. Agora sorteia 1 ou 2.
                int aguaGanho = Criatura.Rng.Next(1, 3);
                jogador.AguaBenta += aguaGanho;
                Console.WriteLine($"Recompensa: +{aguaGanho} Água Benta!");
                break;

            case "chefe":
                jogador.Sal += 3;
                jogador.AguaBenta += 2;
                Console.WriteLine("Recompensa: +3 Sal Grosso e +2 Água Benta!");
                break;

            default:
                // Recompensa padrão: 50% sal, 50% água benta
                if (Criatura.Rng.NextDouble() < 0.5)
                {
                    jogador.Sal += 1;
                    Console.WriteLine("Recompensa: +1 Sal Grosso!");
                }
                else
                {
                    jogador.AguaBenta += 1;
                    Console.WriteLine("Recompensa: +1 Água Benta!");
                }
                break;
        }

        Console.WriteLine($"Inventário atual: Sal({jogador.Sal}) Água({jogador.AguaBenta})");
    }
}

// ============================================================
// HISTÓRIA / FLUXO DO JOGO
// ============================================================
public class Jogo
{
    private Guerreiro jogador;
    private Batalha sistemaBatalha;

    public Jogo()
    {
        jogador = new Guerreiro("Michael Thomas", "Crucifixo Abençoado");
        sistemaBatalha = new Batalha();
    }

    // Percorre os capítulos em ordem; se o jogador morrer, o jogo termina
    public void Iniciar()
    {
        // TÍTULO ALTERADO: "MALDITA" -> "ASSOMBRADA"
        Console.WriteLine(" RUBY - A FLORESTA ASSOMBRADA ");
        Console.WriteLine("=====================================");
        Console.WriteLine("Você é Michael Thomas, em busca de sua amada.");
        Console.WriteLine("A floresta está repleta de criaturas sobrenaturais...");
        Console.WriteLine("Encontre o rubi e liberte a alma de sua esposa!");

        Thread.Sleep(2000);

        // ----- Capítulo 1 -----
        Console.WriteLine("\nCAPÍTULO 1: A FLORESTA NOTURNA");
        Console.WriteLine("Você adentra a floresta densa e sombria...");
        Console.WriteLine("Névoa espessa envolve tudo. Corvos observam...");
        Thread.Sleep(2000);

        if (!ExplorarFloresta()) { FimDeJogo(); return; }

        // ----- Capítulo 2 -----
        Console.WriteLine("\nCAPÍTULO 2: A MANSÃO MAL-ASSOMBRADA");
        Console.WriteLine("Você encontra uma mansão abandonada...");
        Console.WriteLine("Portas rangem e vozes sussurram nas sombras...");
        Thread.Sleep(2000);

        if (!ExplorarMansao()) { FimDeJogo(); return; }

        // ----- Capítulo 3 -----
        Console.WriteLine("\nCAPÍTULO 3: A CIDADE FANTASMA");
        Console.WriteLine("Através da mansão, você encontra uma cidade abandonada...");
        Console.WriteLine("Edifícios em ruínas e silêncio perturbador...");
        Thread.Sleep(2000);

        if (!ExplorarCidade()) { FimDeJogo(); return; }

        // ----- Capítulo final -----
        Console.WriteLine("\nCAPÍTULO FINAL: O ALTAR DE SACRIFÍCIO");
        Console.WriteLine("Você encontra o altar com o rubi...");
        Console.WriteLine("A alma de sua amada está presa dentro da joia!");
        Thread.Sleep(2000);

        EnfrentarChefeFinal();
    }

    // CORREÇÃO: antes, morrer nos capítulos 1-3 encerrava o jogo sem nenhuma mensagem.
    private void FimDeJogo()
    {
        Console.WriteLine("\nGAME OVER");
        Console.WriteLine("As trevas consumiram a floresta...");
        Console.WriteLine("A alma de sua amada permanece presa para sempre...");
    }

    // Capítulo 1: fantasma opcional + baú + chefe Ceifeiro
    private bool ExplorarFloresta()
    {
        Console.WriteLine("Você ouve ruídos na floresta...");
        Console.WriteLine("1. Investigar os ruídos");
        Console.WriteLine("2. Continuar caminhando");
        Console.Write("Escolha: ");

        if (Console.ReadLine() == "1")
        {
            Console.WriteLine("Um Fantasma aparece!");
            Fantasma fantasma = new Fantasma("Alma Peregrina");
            if (!sistemaBatalha.Iniciar(jogador, fantasma, "fantasma")) return false;
        }

        // Baú com itens
        Console.WriteLine("Você encontra um baú antigo!");
        Console.WriteLine("Dentro você encontra: +2 Sal Grosso e +1 Água Benta");
        jogador.Sal += 2;
        jogador.AguaBenta += 1;
        Console.WriteLine($"Inventário: Sal({jogador.Sal}) Água({jogador.AguaBenta})");

        // Luta obrigatória contra o Ceifeiro
        Console.WriteLine("O Ceifeiro aparece na sua frente!");
        Ceifeiro ceifeiro = new Ceifeiro();
        return sistemaBatalha.Iniciar(jogador, ceifeiro, "chefe");
    }

    // Capítulo 2: escolha diante da sombra; pode virar uma luta contra um Demônio
    private bool ExplorarMansao()
    {
        Console.WriteLine("Na sala de jantar, você vê uma sombra...");

        // CORREÇÃO: agora repete a pergunta até receber uma opção válida
        // (antes, qualquer entrada inválida invocava o Demônio sem aviso).
        while (true)
        {
            Console.WriteLine("1. Usar Crucifixo");
            Console.WriteLine("2. Jogar Sal");
            Console.WriteLine("3. Ignorar e continuar");
            Console.Write("Escolha: ");
            string escolha = Console.ReadLine();

            if (escolha == "1")
            {
                Console.WriteLine("O crucifixo brilha, afastando a sombra!");
                break;
            }
            else if (escolha == "2")
            {
                if (jogador.Sal > 0)
                {
                    Console.WriteLine("O sal purifica a área!");
                    jogador.Sal--;
                    break;
                }

                // Sem sal: a sombra percebe a fraqueza e ataca
                Console.WriteLine("Você não tem Sal Grosso! A sombra se transforma em um Demônio e te ataca!");
                Demonio demonio = new Demonio("Demônio da Mansão");
                if (!sistemaBatalha.Iniciar(jogador, demonio, "demonio")) return false;
                break;
            }
            else if (escolha == "3")
            {
                Console.WriteLine("Você decide ignorar a sombra...");
                break;
            }
            else
            {
                Console.WriteLine("Opção inválida! Tente novamente.");
            }
        }

        // Bônus permanente
        Console.WriteLine("Você encontra uma Bíblia antiga!");
        Console.WriteLine("Sua fé é fortalecida! +5 de Ataque permanente");
        jogador.Ataque += 5;
        Console.WriteLine($"Ataque aumentado para: {jogador.Ataque}");

        return true;
    }

    // Capítulo 3: igreja com Água Benta + três fantasmas em sequência
    private bool ExplorarCidade()
    {
        Console.WriteLine("Você encontra uma igreja abandonada...");
        Console.WriteLine("Dentro, encontra frascos de Água Benta!");
        jogador.AguaBenta += 3;
        Console.WriteLine($"Água Benta: {jogador.AguaBenta}");

        Console.WriteLine("Três Fantasmas aparecem!");
        for (int i = 1; i <= 3; i++)
        {
            Fantasma fantasma = new Fantasma($"Fantasma {i}");
            if (!sistemaBatalha.Iniciar(jogador, fantasma, "fantasma")) return false;
            if (i < 3)
            {
                Console.WriteLine("Próximo fantasma se aproxima...");
                Thread.Sleep(1000);
            }
        }

        return true;
    }

    // Batalha final contra o Retorcido e desfecho da história
    private void EnfrentarChefeFinal()
    {
        Console.WriteLine("O Retorcido guarda o rubi!");
        Retorcido retorcido = new Retorcido();

        if (sistemaBatalha.Iniciar(jogador, retorcido, "chefe"))
        {
            Console.WriteLine("\nVOCÊ VENCEU!");
            Console.WriteLine("Michael quebra o rubi e liberta a alma de sua amada!");
            Console.WriteLine("A névoa dissipa e a paz retorna à floresta...");
            Console.WriteLine("FIM - O amor prevaleceu sobre as trevas.");
        }
        else
        {
            FimDeJogo();
        }
    }
}

// ============================================================
// PONTO DE ENTRADA
// ============================================================
class Program
{
    static void Main(string[] args)
    {
        // CORREÇÃO: garante que acentos e caracteres especiais apareçam corretamente no console
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Jogo jogo = new Jogo();
        jogo.Iniciar();

        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}
