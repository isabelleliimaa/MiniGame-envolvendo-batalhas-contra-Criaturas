
private bool ExplorarFloresta()
{
    // Apresenta a situação inicial da floresta e as opções do jogador
    Console.WriteLine("Você ouve ruídos na floresta...");
    Console.WriteLine("1. Investigar os ruídos");
    Console.WriteLine("2. Continuar caminhando");
    Console.Write("Escolha: ");

    // Verifica se o jogador escolheu investigar os ruídos
    if (Console.ReadLine() == "1")
    {
        Console.WriteLine("Um Fantasma aparece!");

        // Cria um fantasma para iniciar a batalha
        Fantasma fantasma = new Fantasma("Alma Peregrina");

        // Se o jogador for derrotado, encerra a exploração
        if (sistemaBatalha.Iniciar(jogador, fantasma, "fantasma") == ResultadoBatalha.Derrota)
            return false;
    }

    // O jogador encontra um baú com itens para o inventário
    Console.WriteLine("Você encontra um baú antigo!");
    Console.WriteLine("Dentro você encontra: +2 Sal Grosso e +1 Água Benta");

    // Adiciona os itens encontrados ao inventário do jogador
    jogador.Sal += 2;
    jogador.AguaBenta += 1;

    // Mostra a quantidade atual de itens
    Console.WriteLine($"Inventário: Sal({jogador.Sal}) Água({jogador.AguaBenta})");

    // Apresenta o chefe desta etapa
    Console.WriteLine("O Ceifeiro aparece na sua frente!");
    Ceifeiro ceifeiro = new Ceifeiro();

    // Inicia a batalha e retorna false caso o jogador seja derrotado
    return sistemaBatalha.Iniciar(jogador, ceifeiro, "chefe") != ResultadoBatalha.Derrota;
}

private bool ExplorarMansao()
{
    // Apresenta a situação e as opções disponíveis na mansão
    Console.WriteLine("Na sala de jantar, você vê uma sombra...");
    Console.WriteLine("1. Usar Crucifixo");
    Console.WriteLine("2. Jogar Sal");
    Console.WriteLine("3. Ignorar e continuar");
    Console.Write("Escolha: ");

    // Armazena a opção digitada pelo jogador
    string escolha = Console.ReadLine();

    // Opção 1: usar o crucifixo para afastar a sombra
    if (escolha == "1")
    {
        Console.WriteLine("O crucifixo brilha, afastando a sombra!");
    }
    // Opção 2: usar sal, desde que exista sal no inventário
    else if (escolha == "2" && jogador.Sal > 0)
    {
        Console.WriteLine("O sal purifica a área!");

        // Consome uma unidade de Sal Grosso
        jogador.Sal--;
    }
    // Opção 3: ignorar a sombra e continuar
    else if (escolha == "3")
    {
        Console.WriteLine("Você decide ignorar a sombra...");
    }
    // Trata uma opção inválida ou a falta de Sal Grosso
    else if ((escolha != "1" && escolha != "2" && escolha != "3") ||
             (escolha == "2" && jogador.Sal <= 0))
    {
        Console.WriteLine("Escolha inválida ou você não tem Sal Grosso. Você segue em frente com cautela.");
    }

    // O jogador encontra uma Bíblia que aumenta seu ataque
    Console.WriteLine("Você encontra uma Bíblia antiga!");
    Console.WriteLine("Sua fé é fortalecida! +5 de Ataque permanente");

    // Aumenta o atributo de ataque em cinco pontos
    jogador.Ataque += 5;

    // Exibe o novo valor de ataque
    Console.WriteLine($"Ataque aumentado para: {jogador.Ataque}");

    // Retorna true para continuar a história
    return true;
}

private bool ExplorarCidade()
{
    // Apresenta a igreja abandonada encontrada na cidade
    Console.WriteLine("Você encontra uma igreja abandonada...");
    Console.WriteLine("Dentro, encontra frascos de Água Benta!");

    // Adiciona três unidades de Água Benta ao inventário
    jogador.AguaBenta += 3;

    // Mostra a quantidade atual de Água Benta
    Console.WriteLine($"Água Benta: {jogador.AguaBenta}");

    // Informa que o jogador enfrentará três fantasmas
    Console.WriteLine("Três Fantasmas aparecem!");

    // Repete a batalha três vezes, criando um fantasma por rodada
    for (int i = 1; i <= 3; i++)
    {
        Fantasma fantasma = new Fantasma($"Fantasma {i}");

        // Encerra a exploração caso o jogador seja derrotado
        if (sistemaBatalha.Iniciar(jogador, fantasma, "fantasma") == ResultadoBatalha.Derrota)
            return false;

        // Faz uma pausa antes da próxima batalha, exceto após a última
        if (i < 3)
        {
            Console.WriteLine("Próximo fantasma se aproxima...");
            Thread.Sleep(1000);
        }
    }

    // Retorna true se o jogador sobreviver às três batalhas
    return true;
}

private void EnfrentarChefeFinal()
{
    // Apresenta o inimigo da batalha final
    Console.WriteLine("O Retorcido guarda o rubi!");
    Retorcido retorcido = new Retorcido();

    // Inicia a batalha e verifica se o resultado foi uma vitória
    if (sistemaBatalha.Iniciar(jogador, retorcido, "chefe") == ResultadoBatalha.Vitoria)
    {
        // Exibe as mensagens de conclusão da história
        Console.WriteLine("VOCÊ VENCEU!");
        Console.WriteLine("Michael quebra o rubi e liberta a alma de sua amada!");
        Console.WriteLine("A névoa dissipa e a paz retorna à floresta...");
        Console.WriteLine("FIM - O amor prevaleceu sobre as trevas.");
    }
    else
    {
        // Exibe o final alternativo caso o jogador não vença
        Console.WriteLine("GAME OVER");
        Console.WriteLine("As trevas consumiram a floresta...");
        Console.WriteLine("A alma de sua amada permanece presa para sempre...");
    }
}
}

// Ponto de entrada do programa
class Program
{
    static void Main(string[] args)
    {
        // Cria uma nova instância do jogo
        Jogo jogo = new Jogo();

        // Inicia a história e a sequência de eventos
        jogo.Iniciar();

        // Aguarda uma tecla antes de fechar o console
        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}
